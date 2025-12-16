import datetime
from flask import Blueprint, jsonify, render_template, request, Flask
from prophet import Prophet
import pandas as pd
from datetime import datetime, timedelta
import numpy as np

from S5_01_Flask_CS_GOAT.services.model import PriceHistory, Wear, db



ia_bp = Blueprint('ia', __name__)


def predict_and_save(skin_id, wear_id, jours):
    
    price_historie = get_by_id(wear_id)
    if price_historie is None or len(price_historie) < 10:
        return None

    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue']
    } for ph in price_historie])

    print("="*80)
    print("Epoch no:")
    print(df.to_string())
    print("="*80)
    
    df = df.sort_values('ds')
    
    split_index = int(len(df) * 0.75)
    train_df = df[:split_index]
    
    model = Prophet(
        yearly_seasonality=False,
        weekly_seasonality=False,
        daily_seasonality=False,
        changepoint_prior_scale=0.05,
    )
    print("Training the model...")
    
    model.fit(train_df)
    last_date = df['ds'].max()
    future_dates = [last_date + timedelta(days=i) for i in range(1, jours + 1)]
    future_df = pd.DataFrame({'ds': future_dates})
    
    print(f"Predicting for {len(future_dates)} days: {future_dates[0]} to {future_dates[-1]}")
    
    # ← Prédiction pour tous les jours
    forecast = model.predict(future_df)
    
    print("="*80)
    print("PREDICTIONS:")
    print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']])
    print("="*80)
    
    wear = Wear.query.get_or_404(wear_id)
    
    for idx, row in forecast.iterrows():
        new_prediction = PriceHistory(
            skin_id=skin_id,
            wear_type_id=wear.wear_type_id,
            price_value=round(float(row['yhat']), 2),
            price_date=row['ds'],
            guess_date=datetime.now(),
        )
        db.session.add(new_prediction)
        print(f"Day {idx+1}: {row['ds'].date()} → {row['yhat']:.2f}€")
    
    db.session.commit()
    
    print(f"Saved {len(forecast)} predictions!")
    
    return True



def get_by_id(wear_id):
    wear = Wear.query.get_or_404(wear_id)
    print("Wear found:", wear.wear_id, "skin_id:", wear.skin_id, "wear_type_id:", wear.wear_type_id)
    price_histories = PriceHistory.query.filter_by(
        skin_id=wear.skin_id,
        wear_type_id=wear.wear_type_id,
        guess_date=None,
    ).all()
    print(f"Found {len(price_histories)} price histories for skin_id={wear.skin_id} and wear_type_id={wear.wear_type_id}")
    return [ph.to_dict() for ph in price_histories]


def get_all(skin_id, wear_type_id):
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id
    ).all()

    print(f"Found {len(price_histories)} price histories for skin_id={skin_id} and wear_type_id={wear_type_id}")
    return [ph.to_dict() for ph in price_histories]