from flask import Blueprint
from prophet import Prophet
import pandas as pd
from datetime import datetime, timedelta
import numpy as np
import joblib
import os
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from S5_01_Flask_CS_GOAT.services.model import PriceHistory, Wear, Skin, db
from S5_01_Flask_CS_GOAT import debug, debug_print
from enum import IntEnum


ia_bp = Blueprint('ia', __name__)

class Seasonality(IntEnum):
    DAILY = 7
    WEEKLY = 14
    YEARLY = 365

MODELS_DIR = os.path.join(os.path.dirname(__file__), 'models')
os.makedirs(MODELS_DIR, exist_ok=True)

def predict_and_save(jours:int=30, wear_id:int=None, skin_id:int=None, wear_type_id:int=None) -> bool:
    if jours > 30:
        debug_print(f"Limiting prediction from {jours} to 30 days for better accuracy")
        jours = 30
    
    if wear_id:
        (skin_id, wear_type_id) = predict_with_wear(wear_id)
    if skin_id is None or wear_type_id is None:
        return None
    debug_print(f"Wear found {wear_id} skin_id: {skin_id}, wear_type_id: {wear_type_id}")
    return do_predict(skin_id, wear_type_id, jours)


def check_seasonality(training_days: int = 7) -> tuple[bool, bool, bool]:
    yearly: bool = False
    weekly: bool = False
    daily: bool = False

    if(training_days < Seasonality.DAILY):
        training_days = Seasonality.DAILY

    if training_days >= Seasonality.DAILY:
        daily = True
    if training_days >= Seasonality.WEEKLY:
        weekly = True
    if training_days >= Seasonality.YEARLY:
        yearly = True
                    
    return yearly, weekly, daily

def predict_with_wear(wear_id:int) -> tuple[int,int]:
    if wear_id:
        wear = Wear.query.get(wear_id)
        if wear is None:
            debug_print(f"Wear {wear_id} not found")
            return None, None
        skin_id = wear.skin_id
        wear_type_id = wear.wear_type_id
        return skin_id, wear_type_id

def get_item_id_by_skin(skin_id: int) -> int:
    skin = Skin.query.get(skin_id)
    debug_print(f"{skin}")
    if skin is None:
        return None
    debug_print(f"found item_id:{skin.item_id} for skin:{skin_id}")
    return skin.item_id

def get_model_path(item_id: int) -> str:
    return os.path.join(MODELS_DIR, f'model_item_{item_id}.pkl')

def load_model(item_id: int):
    model_path = get_model_path(item_id)
    
    if not os.path.exists(model_path):
        return None
    
    model = joblib.load(model_path)
    debug_print(f"Loaded model from {model_path}")
    return model

def save_model(model, item_id: int):
    model_path = get_model_path(item_id)
    joblib.dump(model, model_path)
    debug_print(f"Saved model to {model_path}")

def apply_dampening(forecast: pd.DataFrame, last_price: float, max_daily_change: float = 0.02) -> pd.DataFrame:
    forecast = forecast.copy()
    
    for i in range(len(forecast)):
        if i == 0:
            max_increase = last_price * (1 + max_daily_change)
            max_decrease = last_price * (1 - max_daily_change)
            forecast.loc[forecast.index[i], 'yhat'] = np.clip(
                forecast.loc[forecast.index[i], 'yhat'],
                max_decrease,
                max_increase
            )
        else:
            prev_price = forecast.loc[forecast.index[i-1], 'yhat']
            max_increase = prev_price * (1 + max_daily_change)
            max_decrease = prev_price * (1 - max_daily_change)
            forecast.loc[forecast.index[i], 'yhat'] = np.clip(
                forecast.loc[forecast.index[i], 'yhat'],
                max_decrease,
                max_increase
            )
    
    return forecast

def do_predict(skin_id:int, wear_type_id:int, jours:int, training_days:int=60) -> bool:
    jours = min(jours, 30)

    item_id = get_item_id_by_skin(skin_id)
    if item_id is None:
        return False

    price_history = get_all(skin_id, wear_type_id)
    
    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue'],
        'volume': ph.get('volume', 0) or 0
    } for ph in price_history])

    df = df.sort_values('ds')
    
    cutoff_date = datetime.now() - timedelta(days=training_days)
    df_train = df[df['ds'] >= cutoff_date].copy()
    
    if len(df_train) < 29:
        debug_print(f"Not enough recent data: {len(df_train)} points")
        return False
    
    max_price = df['y'].quantile(0.95)
    price_ceiling = max_price * 1.3
    price_floor = df_train['y'].quantile(0.05)
    
    df_train['cap'] = price_ceiling
    df_train['floor'] = price_floor
    
    debug_print(f"Training on {len(df_train)} recent data points")
    debug_print(f"Price range: {df_train['y'].min():.2f}€ - {df_train['y'].max():.2f}€")
    debug_print(f"Last 5 prices: {df_train['y'].tail(5).tolist()}")
    debug_print(f"Last price: {df_train['y'].iloc[-1]:.2f}€")
    debug_print(f"Price ceiling: {price_ceiling:.2f}€, floor: {price_floor:.2f}€")
    
    yearly, weekly, daily = check_seasonality(training_days)

    model = Prophet(
        growth='logistic',
        yearly_seasonality=False,
        weekly_seasonality=True,
        daily_seasonality=False,
        changepoint_prior_scale=0.01,
        seasonality_prior_scale=0.5,
        interval_width=0.80
    )
    
    model.add_regressor('volume')
    model.fit(df_train)
    save_model(model, item_id)

    last_historical_date = df_train['ds'].max()
    last_historical_price = df_train['y'].iloc[-1]
    
    start_prediction = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
    future_dates = [start_prediction + timedelta(days=i) for i in range(jours)]
    
    recent_volume = df_train['volume'].tail(7).mean()
    
    future_df = pd.DataFrame({
        'ds': future_dates,
        'volume': [recent_volume] * jours,
        'cap': [price_ceiling] * jours,
        'floor': [price_floor] * jours
    })
    
    forecast = model.predict(future_df)
    
    forecast = apply_dampening(forecast, last_historical_price, max_daily_change=0.03)
    
    first_prediction = forecast['yhat'].iloc[0]
    gap = first_prediction - last_historical_price
    gap_percent = abs(gap) / last_historical_price
    
    debug_print(f"Last historical: {last_historical_price:.2f}€")
    debug_print(f"First prediction: {first_prediction:.2f}€")
    debug_print(f"Gap: {gap:.2f}€ ({gap_percent*100:.1f}%)")
    
    if gap_percent > 0.15:
        debug_print("Large gap detected, applying smooth transition...")
        
        transition_days = min(7, len(forecast))
        transition_weights = np.linspace(0, 1, transition_days)
        
        for i in range(transition_days):
            adjusted = last_historical_price * (1 - transition_weights[i]) + \
                      forecast['yhat'].iloc[i] * transition_weights[i]
            
            gap_adjustment = forecast['yhat'].iloc[i] - adjusted
            
            forecast.loc[forecast.index[i], 'yhat'] = adjusted
            forecast.loc[forecast.index[i], 'yhat_lower'] -= gap_adjustment * 0.8
            forecast.loc[forecast.index[i], 'yhat_upper'] -= gap_adjustment * 0.8
    
    if debug:
        draw_debug_graph(df_train, forecast, skin_id, wear_type_id, item_id)

    for idx, row in forecast.iterrows():
        new_prediction = PriceHistory(
            skin_id=skin_id,
            wear_type_id=wear_type_id,
            price_value=round(float(row['yhat']), 2),
            price_date=row['ds'],
            guess_date=datetime.now(),
            volume=1        
        )
        if not debug:
            db.session.add(new_prediction)
    
    if not debug: 
        db.session.commit()
    
    print(f"Saved {len(forecast)} predictions!")
    
    return True

def draw_debug_graph(df_train: pd.DataFrame, forecast: pd.DataFrame, skin_id: int, wear_type_id: int, item_id: int) -> bool:
    lookback_days = 60
    cutoff_date = datetime.now() - timedelta(days=lookback_days)
    df_recent = df_train[df_train['ds'] >= cutoff_date].copy()
    
    fig, ax = plt.subplots(figsize=(16, 8))
    
    ax.plot(df_recent['ds'], df_recent['y'], 'o-', color='#1f77b4', label='Historical Data', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    last_historical_date = df_recent['ds'].max()
    last_historical_price = df_recent['y'].iloc[-1]
    first_prediction_date = forecast['ds'].iloc[0]
    first_prediction_price = forecast['yhat'].iloc[0]
    
    if last_historical_date < first_prediction_date:
        ax.plot([last_historical_date, first_prediction_date], 
                [last_historical_price, first_prediction_price], 
                '--', color='gray', linewidth=1.5, alpha=0.5, label='Gap')
    
    ax.plot(forecast['ds'], forecast['yhat'], 'o-', color='#ff7f0e', label='Predictions', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    ax.fill_between(forecast['ds'], 
                     forecast['yhat_lower'], 
                     forecast['yhat_upper'], 
                     alpha=0.2, 
                     color='#ff7f0e', 
                     label='Confidence Interval')
    
    today = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
    ax.axvline(x=today, color='#2ca02c', linestyle='--', linewidth=2, alpha=0.7, label='Today')
    
    ax.set_xlabel('Date', fontsize=14, fontweight='bold')
    ax.set_ylabel('Price (€)', fontsize=14, fontweight='bold')
    ax.set_title(f'Price Prediction - Skin ID: {skin_id}, Wear Type: {wear_type_id}\n(Last {lookback_days} days + {len(forecast)} days forecast)', 
              fontsize=16, fontweight='bold')
    ax.legend(loc='best', fontsize=12, framealpha=0.95)
    ax.grid(True, alpha=0.3, linestyle='--')
    
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%d/%m'))
    ax.xaxis.set_major_locator(mdates.DayLocator(interval=max(1, (lookback_days + len(forecast)) // 15)))
    plt.xticks(rotation=45, ha='right', fontsize=11)
    plt.yticks(fontsize=11)
    
    plt.tight_layout()
    
    graphs_dir = os.path.join(os.path.dirname(__file__), 'graphs')
    os.makedirs(graphs_dir, exist_ok=True)
    
    graph_path = os.path.join(graphs_dir, f'prediction_item_{item_id}_skin_{skin_id}_wear_{wear_type_id}.png')
    plt.savefig(graph_path, dpi=200, bbox_inches='tight')
    debug_print(f"Graph saved: {graph_path}")
    
    plt.close()
    return True


def get_all(skin_id:int, wear_type_id:int) -> list:
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id,
        guess_date=None,
    ).all()

    return [ph.to_dict() for ph in price_histories]