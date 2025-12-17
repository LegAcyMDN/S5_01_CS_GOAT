import datetime
from flask import Blueprint, jsonify, current_app
from prophet import Prophet
import pandas as pd
from datetime import datetime, timedelta
import numpy as np
from S5_01_Flask_CS_GOAT.services.model import PriceHistory, Wear, db
from S5_01_Flask_CS_GOAT import debug 

ia_bp = Blueprint('ia', __name__)



def predict_and_save_by_wear(wear_id, jours=30):
    wear = Wear.query.get(wear_id)
    print("wear_type_id found ", wear.wear_type_id)
    if wear is None:
        return None
    
    return _predict_and_save_internal(wear.skin_id, wear.wear_type_id, jours)

def predict_and_save_by_skin_wear(skin_id, wear_type_id, jours=30):
    return _predict_and_save_internal(skin_id, wear_type_id, jours)

def _predict_and_save_internal(skin_id, wear_type_id, jours, training_days=7):

    
    if debug:
        print("skin_id:", skin_id)
        print("wear_type_id:", wear_type_id)
    
    price_history = get_all(skin_id, wear_type_id)
    if price_history is None or len(price_history) < training_days:
        return None

    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue'],
        'volume': ph.get('volume', 0) or 0
    } for ph in price_history])

    df = df.sort_values('ds')

    if len(df) < training_days:
        return None
    
    if debug:
        print("="*80)
        print(f"Training data (last {training_days} days from today):")
        print(df.to_string())
        print("="*80)

    model = Prophet(
        yearly_seasonality=False,
        weekly_seasonality=False,
        daily_seasonality=False,
        changepoint_prior_scale=0.05,
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
    
    if debug:
        print(f"Predicting for {len(future_dates)} days: {future_dates[0]} to {future_dates[-1]}")
        print(f"Using average volume: {avg_volume:.2f}")
    
    forecast = model.predict(future_df)
    if debug:
        print("="*80)
        print("PREDICTIONS:")
        print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])
        print("="*80)

    for idx, row in forecast.iterrows():
        new_prediction = PriceHistory(
            skin_id=skin_id,
            wear_type_id=wear_type_id,
            price_value=round(float(row['yhat']), 2),
            price_date=row['ds'],
            guess_date=datetime.now(),
            volume=1
        )
        db.session.add(new_prediction)
        if debug:
            print(f"Day {idx+1}: {row['ds'].date()} → {row['yhat']:.2f}€")
    
    db.session.commit()
    
    print(f"Saved {len(forecast)} predictions!")
    
    return True

def get_all(skin_id, wear_type_id):
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id,
        guess_date=None,
    ).all()

    return [ph.to_dict() for ph in price_histories]
