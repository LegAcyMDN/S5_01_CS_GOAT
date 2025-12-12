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
    
    if not price_historie or len(price_historie) < 10:
        return None
    
    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue']
    } for ph in price_historie])
    
    df = df.sort_values('ds')
    
    split_index = int(len(df) * 0.75)
    train_df = df[:split_index]
    #test_df = df[split_index:]
    
    model = Prophet(
        yearly_seasonality=True,
        weekly_seasonality=True,
        daily_seasonality=False,
        changepoint_prior_scale=0.05
    )
    
    model.fit(train_df)
    
    # test_forecast = model.predict(test_df[['ds']])
    # mae = np.mean(np.abs(test_forecast['yhat'] - test_df['y']))
    
    future_date = datetime.now() + timedelta(days=jours)
    future_df = pd.DataFrame({'ds': [future_date]})
    
    forecast = model.predict(future_df)
    predicted_price = forecast['yhat'].values[0]
    
    new_prediction = PriceHistory(
        skin_id=skin_id,
        wear_type_id=price_historie[0]['weartypeid'],
        price_value=float(predicted_price),
        price_date=future_date,
        guess_date=datetime.now()
    )
    
    db.session.add(new_prediction)
    db.session.commit()
    
    return



def get_by_id(wear_id):
    
    wear = Wear.query.get_or_404(wear_id)
    return get_all(wear.skin_id, wear.wear_type_id)


def get_all(skin_id, wear_type_id):
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id
    ).all()
    
    if not price_histories:
        return None
    
    return [ph.to_dict() for ph in price_histories]
