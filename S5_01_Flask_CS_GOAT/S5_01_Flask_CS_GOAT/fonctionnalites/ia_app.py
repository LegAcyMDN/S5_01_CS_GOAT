from flask import Blueprint
from prophet import Prophet
import pandas as pd
from datetime import datetime, timedelta
import numpy as np
from S5_01_Flask_CS_GOAT.services.model import PriceHistory, Wear, db
from S5_01_Flask_CS_GOAT import debug, PRINT
from enum import IntEnum


ia_bp = Blueprint('ia', __name__)

class Seasonality(IntEnum):
    DAILY = 7
    WEEKLY = 14
    YEARLY = 365

def predict_and_save(jours:int=30, wear_id:int=None, skin_id:int=None, wear_type_id:int=None) -> bool:
    if wear_id:
        (skin_id, wear_type_id) = predict_with_wear(wear_id)
    if skin_id is None or wear_type_id is None:
        return None
    PRINT(f"Wear found {wear_id} skin_id: {skin_id}, wear_type_id: {wear_type_id}")
    return _predict_and_save_internal(skin_id, wear_type_id, jours)


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
            PRINT(f"Wear {wear_id} not found")
            return None, None
        skin_id = wear.skin_id
        wear_type_id = wear.wear_type_id
        return skin_id, wear_type_id

def _predict_and_save_internal(skin_id:int, wear_type_id:int, jours:int, training_days:int=7) -> bool:

    price_history = get_all(skin_id, wear_type_id)
    if len(price_history) < training_days:
        PRINT(f"Not enough data to train the model for skin_id: {skin_id}, wear_type_id: {wear_type_id}. Required: {training_days}, Available: {len(price_history)}")
        return False

    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue'],
        'volume': ph.get('volume', 0) or 0
    } for ph in price_history])

    df = df.sort_values('ds')
    
    PRINT("="*80)
    PRINT(f"Training data (last {training_days} days from today):")
    PRINT(df.to_string())
    PRINT("="*80)

    yearly, weekly, daily = check_seasonality(training_days)

    model = Prophet(
        yearly_seasonality=yearly,
        weekly_seasonality=weekly,
        daily_seasonality=daily,
        changepoint_prior_scale=0.5,
    )
    
    model.add_regressor('volume')
    
    print("Training the model with volume regressor...")

    model.fit(df)

    tomorrow = datetime.now() + timedelta(days=1)
    tomorrow = tomorrow.replace(hour=0, minute=0, second=0, microsecond=0)
    future_dates = [tomorrow + timedelta(days=i) for i in range(jours)]
    
    avg_volume = df['volume'].mean()
    
    future_df = pd.DataFrame({
        'ds': future_dates,
        'volume': [avg_volume] * jours
    })
    
    PRINT(f"Predicting for {len(future_dates)} days: {future_dates[0]} to {future_dates[-1]}")
    PRINT(f"Using average volume: {avg_volume:.2f}")
    
    forecast = model.predict(future_df)
   
    PRINT("="*80)
    PRINT("PREDICTIONS:")
    PRINT(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])
    PRINT("="*80)

    for idx, row in forecast.iterrows():
        new_prediction = PriceHistory(
            skin_id=skin_id,
            wear_type_id=wear_type_id,
            price_value=round(float(row['yhat']), 2),
            price_date=row['ds'],
            guess_date=datetime.now(),
            volume=1        
        )
        PRINT(new_prediction)
        if debug:
            db.session.add(new_prediction)
        
        PRINT(f"Day {idx+1}: {row['ds'].date()} → {row['yhat']:.2f}€")
    
    if not debug: 
        db.session.commit()
    
    print(f"Saved {len(forecast)} predictions!")
    
    return True

def get_all(skin_id:int, wear_type_id:int) -> list:
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id,
        guess_date=None,
    ).all()

    return [ph.to_dict() for ph in price_histories]