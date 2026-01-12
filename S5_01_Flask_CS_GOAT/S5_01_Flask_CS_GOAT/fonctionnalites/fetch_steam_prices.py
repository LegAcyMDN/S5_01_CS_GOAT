import requests
import psycopg2
from datetime import datetime, date, timedelta
import random
from time import sleep
from dotenv import load_dotenv
import os
from S5_01_Flask_CS_GOAT import debug_print

import urllib

# Load environment variables from .env file
load_dotenv()

STEAM_LOGIN_SECURE = os.getenv("STEAM_LOGIN_SECURE")
APPID_CS2 = int(os.getenv("APPID_CS2", 730))


def get_price_history(item_name, appid=APPID_CS2):
    """
    Fetch price history from Steam.
    """
    
    base_url = "https://steamcommunity.com/market/pricehistory"
    params = {
        "appid": appid,
        "market_hash_name": item_name,
    }

    cookies = {
        "steamLoginSecure": STEAM_LOGIN_SECURE,
    }

    headers = {
        "User-Agent": "Mozilla/5.0",
        "Referer": "https://steamcommunity.com/market/",
        "Accept": "application/json,text/javascript,*/*;q=0.01",
    }

    resp = requests.get(base_url, params=params, cookies=cookies, headers=headers, timeout=30)
    resp.raise_for_status()
    return resp.json()


def get_wear_type_id_from_name(connection, wear_name):
    """
    Retrieve the wear_type_id for a given wear type name from the database.
    """
    cursor = connection.cursor()
    cursor.execute(
        """
        SELECT wrt_id
        FROM t_e_weartype_wrt
        WHERE wrt_weartypename = %s
        """,
        (wear_name,)
    )
    result = cursor.fetchone()
    cursor.close()

    if result:
        return result[0]
    else:
        debug_print(f"No wear_type_id found for wear_name: {wear_name}")
        return None


def get_skin_id(connection, item_name):
    """
    Retrieve the skin_id for a given item_name from the database.
    Parse the item_name to extract the part after the pipe (|) and before the parentheses.
    """
    # Extract the part after the pipe and before the parentheses
    if '|' in item_name and '(' in item_name:
        skin_name = item_name.split('|')[1].split('(')[0].strip()
    else:
        skin_name = item_name

    cursor = connection.cursor()
    cursor.execute(
        """
        SELECT skn_id
        FROM t_e_skin_skn
        WHERE skn_skinname = %s
        """,
        (skin_name,)
    )
    result = cursor.fetchone()
    cursor.close()

    if result:
        return result[0]
    else:
        debug_print(f"No skin_id found for item_name: {skin_name}")
        return None


def extract_wear_type_from_item_name(item_name):
    """
    Extract the wear type from the item name.
    Example: "AK-47 | Redline (Field-Tested)" -> "Field-Tested"
    """
    if '(' in item_name and ')' in item_name:
        wear_type = item_name.split('(')[1].split(')')[0].strip()
        return wear_type
    return None


def insert_price_data(connection, item_name, price_data):
    """
    Insert price data into the database.
    """
    cursor = connection.cursor()
    today = date.today()
    seven_days_ago = today - timedelta(days=7)

    skin_id = get_skin_id(connection, item_name)
    if not skin_id:
        debug_print(f"Cannot insert price data. No skin_id found for {item_name}.")
        return

    # Extract wear type from item name
    wear_name = extract_wear_type_from_item_name(item_name)
    wear_type_id = None
    
    if wear_name:
        wear_type_id = get_wear_type_id_from_name(connection, wear_name)
        if not wear_type_id:
            debug_print(f"Cannot insert price data. No wear_type_id found for {wear_name}.")
            return
    else:
        debug_print(f"No wear type found in item name: {item_name}")
        return

    aggregated_data = {}

    for date_str, price_str, volume_str in price_data:
        price_date = None
        try:
            debug_print(f"Processing date: {date_str}, price: {price_str}, volume: {volume_str}")
            cleaned_date_str = date_str.split('+')[0].strip()
            if cleaned_date_str.endswith(":"):
                cleaned_date_str += "00"
            price_date = datetime.strptime(cleaned_date_str, "%b %d %Y %H:%M")
        except ValueError:
            debug_print(f"Skipping invalid date format: {date_str}")
            continue

        if not price_str or not volume_str:
            debug_print(f"Skipping incomplete price data: {date_str}, {price_str}, {volume_str}")
            continue

        if price_date.date() < date(2025, 12, 10):
            debug_print(f"Skipping data before December 10, 2025: {date_str}")
            continue

        try:
            price_value = float(price_str.replace(",", ".")) if isinstance(price_str, str) else float(price_str)
            volume = int(volume_str)
        except ValueError:
            debug_print(f"Skipping invalid price or volume value: {price_str}, {volume_str}")
            continue

        day_key = price_date.date()
        if day_key not in aggregated_data:
            aggregated_data[day_key] = {"total_price": 0, "total_volume": 0}

        aggregated_data[day_key]["total_price"] += price_value * volume
        aggregated_data[day_key]["total_volume"] += volume

    for day_key in aggregated_data:
        aggregated_data[day_key]["entry_count"] = aggregated_data[day_key].get("entry_count", 0) + 1

    for day, data in aggregated_data.items():
        if data["total_volume"] > 0:
            average_price = data["total_price"] / data["total_volume"]
            average_volume = data["total_volume"] / data["entry_count"]

            cursor.execute(
                """
                SELECT 1 FROM t_e_pricehistory_prh
                WHERE prh_pricedate = %s AND skn_id = %s AND wrt_id = %s
                """,
                (datetime.combine(day, datetime.min.time()), skin_id, wear_type_id)
            )
            existing_entry = cursor.fetchone()

            if existing_entry:
                debug_print(f"Skipping already inserted data for {day}")
                continue

            debug_print(f"Inserting aggregated data for {day}: average price {average_price}, average volume {average_volume}, wear_type_id: {wear_type_id}")

            cursor.execute(
                """
                INSERT INTO t_e_pricehistory_prh (prh_pricedate, prh_pricevalue, prh_volume, skn_id, wrt_id)
                VALUES (%s, %s, %s, %s, %s)
                """,
                (datetime.combine(day, datetime.min.time()), average_price, average_volume, skin_id, wear_type_id),
            )

    connection.commit()
    cursor.close()


def get_database_connection():
    """
    Establish a database connection using environment variables.
    """
    return psycopg2.connect(
        database=os.getenv("DB_NAME"),
        user=os.getenv("DB_USER"),
        password=os.getenv("DB_PASSWORD"),
        host=os.getenv("DB_HOST"),
        port=int(os.getenv("DB_PORT", 5432))
    )


def main(item_name=None):
    connection = get_database_connection()

    debug_print(f"Fetching price history for {item_name}")

    try:
        history = get_price_history(item_name)
        if history.get("success") and history.get("prices"):
            insert_price_data(connection, item_name, history["prices"])
            debug_print("Data inserted successfully.")
        else:
            debug_print("No price history found.")
    except Exception as e:
        debug_print(f"Error: {e}")
    finally:
        connection.close()


if __name__ == "__main__":
    main()