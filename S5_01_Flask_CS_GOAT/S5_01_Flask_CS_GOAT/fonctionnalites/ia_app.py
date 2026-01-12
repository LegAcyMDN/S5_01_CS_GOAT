from flask import Blueprint
from prophet import Prophet
import pandas as pd
from datetime import datetime, timedelta
import numpy as np
import joblib
import os
import matplotlib.pyplot as plt
import matplotlib.dates as mdates
from S5_01_Flask_CS_GOAT.services.model import PriceHistory, Wear, Skin,Item,WearType, db
from S5_01_Flask_CS_GOAT import debug, debug_print
from enum import IntEnum
from scipy.stats import linregress
from S5_01_Flask_CS_GOAT.fonctionnalites.fetch_steam_prices import main as fetch_price_data
import urllib.parse

os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'

import tensorflow as tf
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout
from tensorflow.keras.callbacks import EarlyStopping
from tensorflow.keras.regularizers import l2
from sklearn.preprocessing import MinMaxScaler

ia_bp = Blueprint('ia', __name__)

MODELS_DIR = os.path.join(os.path.dirname(__file__), 'models')
os.makedirs(MODELS_DIR, exist_ok=True)

def get_item_name(skin_id: int = None, wear_id: int = None) -> str:
    if skin_id:
        skin = Skin.query.get(skin_id)
        if not skin:
            return None

        item = Item.query.get(skin.item_id)
        if not item:
            return None
        
        return f"{item.item_name} | {skin.skin_name}"

    elif wear_id:
        wear = Wear.query.get(wear_id)
        if not wear:
            return None

        skin = Skin.query.get(wear.skin_id)
        if not skin:
            return None

        item = Item.query.get(skin.item_id)
        wear_type = WearType.query.get(wear.wear_type_id)
        if not item or not wear_type:
            return None

        return f"{item.item_name} | {skin.skin_name} ({wear_type.wear_type_name})"

    return None




def predict_and_save(jours:int=30, wear_id:int=None, skin_id:int=None, wear_type_id:int=None) -> bool:
    if jours > 30:
        debug_print(f"Limiting prediction from {jours} to 30 days for better accuracy")
        jours = 30

    item_name = get_item_name(skin_id=skin_id, wear_id=wear_id)
    if not item_name:
        debug_print("Item name could not be determined.")
        return False
    
    debug_print("Fetching the latest price data...")
    fetch_price_data(item_name)

    if wear_id:
        (skin_id, wear_type_id) = predict_with_wear(wear_id)
    if skin_id is None or wear_type_id is None:
        return None
    debug_print(f"Wear found {wear_id} skin_id: {skin_id}, wear_type_id: {wear_type_id}")
    return do_predict(skin_id, wear_type_id, jours)

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

def get_lstm_model_path(item_id: int) -> str:
    return os.path.join(MODELS_DIR, f'lstm_model_item_{item_id}.h5')

def get_scaler_path(item_id: int) -> str:
    return os.path.join(MODELS_DIR, f'scaler_item_{item_id}.pkl')

def save_model(model, item_id: int):
    model_path = get_model_path(item_id)
    joblib.dump(model, model_path)
    debug_print(f"Saved model to {model_path}")

def save_lstm_model(model, item_id: int):
    model_path = get_lstm_model_path(item_id)
    model.save(model_path)
    debug_print(f"Saved LSTM model to {model_path}")

def save_scaler(scaler, item_id: int):
    scaler_path = get_scaler_path(item_id)
    joblib.dump(scaler, scaler_path)
    debug_print(f"Saved scaler to {scaler_path}")

def create_lstm_sequences(data, lookback=7):
    X, y = [], []
    for i in range(lookback, len(data)):
        X.append(data[i-lookback:i])
        y.append(data[i])
    return np.array(X), np.array(y)

def build_lstm_model(lookback=7):
    model = Sequential([
        LSTM(50, activation='tanh', return_sequences=True, input_shape=(lookback, 1), 
             kernel_regularizer=l2(0.001)),
        Dropout(0.2),
        LSTM(25, activation='tanh', return_sequences=False, kernel_regularizer=l2(0.001)),
        Dropout(0.2),
        Dense(12, activation='relu', kernel_regularizer=l2(0.001)),
        Dense(1)
    ])
    model.compile(optimizer='adam', loss='huber', metrics=['mae'])
    return model

def train_lstm_model(df_train, item_id, lookback=7):
    prices = df_train['y'].values.reshape(-1, 1)
    
    # Use price differences instead of absolute prices for better learning
    price_diffs = np.diff(prices.flatten())
    price_diffs = np.insert(price_diffs, 0, 0)  # Add 0 at the beginning
    
    scaler = MinMaxScaler(feature_range=(-1, 1))
    prices_scaled = scaler.fit_transform(price_diffs.reshape(-1, 1))
    
    if len(prices_scaled) < lookback + 10:
        debug_print(f"Not enough data for LSTM: {len(prices_scaled)} points")
        return None, None
    
    X, y = create_lstm_sequences(prices_scaled, lookback)
    
    debug_print(f"LSTM training data: X shape {X.shape}, y shape {y.shape}")
    debug_print(f"Price range: {prices.min():.4f}€ - {prices.max():.4f}€")
    
    model = build_lstm_model(lookback)
    
    early_stop = EarlyStopping(monitor='loss', patience=15, restore_best_weights=True, min_delta=0.0001)
    
    debug_print("Fitting LSTM model...")
    debug_print("="*80)
    history = model.fit(X, y, epochs=200, batch_size=16, verbose=1, callbacks=[early_stop], validation_split=0.1)
    debug_print("="*80)
    debug_print(f"Training completed after {len(history.history['loss'])} epochs")
    debug_print(f"Final loss: {history.history['loss'][-1]:.6f}")
    
    save_lstm_model(model, item_id)
    save_scaler(scaler, item_id)
    
    # Return both model, scaler, and the actual prices for prediction
    return model, scaler, prices.flatten()

def predict_lstm(model, scaler, last_sequence, last_prices, jours, lookback=7):
    predictions = []
    current_sequence = last_sequence.copy()
    current_price = last_prices[-1]
    
    for i in range(jours):
        
        pred_diff_scaled = model.predict(current_sequence.reshape(1, -1, 1), verbose=0)
        pred_diff = scaler.inverse_transform(pred_diff_scaled)[0, 0]
        
        
        pred_price = current_price + pred_diff
       
        noise = np.random.normal(0, abs(pred_diff) * 0.1)
        pred_price += noise
        
        predictions.append(pred_price)
        current_price = pred_price
        
        
        current_sequence = np.append(current_sequence[1:], pred_diff_scaled[0, 0])
    
    return np.array(predictions)

def do_predict(skin_id:int, wear_type_id:int, jours:int = 30, training_days:int=30) -> bool:
    item_id = get_item_id_by_skin(skin_id)
    if item_id is None:
        return False

    price_history = get_all(skin_id, wear_type_id)
    
    df = pd.DataFrame([{
        'ds': datetime.fromisoformat(ph['pricedate']),
        'y': ph['pricevalue']
    } for ph in price_history])

    df = df.sort_values('ds')
    
    cutoff_date = datetime.now() - timedelta(days=training_days)
    df_train = df[df['ds'] >= cutoff_date].copy()
    df_train = df_train.reset_index(drop=True)
    
    if len(df_train) < 14:
        debug_print(f"Not enough recent data: {len(df_train)} points")
        return False
    
    last_historical_price = df_train['y'].iloc[-1]
    last_historical_date = df_train['ds'].max()
    
    real_data_max = df_train['y'].max()
    real_data_min = df_train['y'].min()
    price_range = real_data_max - real_data_min
    price_ceiling = real_data_max * 1.3 
    price_floor = max(0.01, real_data_min * 0.7)
    
    df_train['cap'] = price_ceiling
    df_train['floor'] = price_floor
    
    debug_print(f"Training on {len(df_train)} data points (REAL data only)")
    debug_print(f"Last historical price: {last_historical_price:.4f}€")
    debug_print(f"Last historical date: {last_historical_date}")
    debug_print(f"Real data max: {real_data_max:.4f}€, min: {real_data_min:.4f}€")
    debug_print(f"Price range: {price_range:.4f}€")
    debug_print(f"Price ceiling: {price_ceiling:.4f}€, floor: {price_floor:.4f}€")
    
    model = Prophet(
        growth='linear',  
        yearly_seasonality=False,
        weekly_seasonality=True,  
        daily_seasonality=False,
        changepoint_prior_scale=0.05, 
        seasonality_prior_scale=1.0,  
        interval_width=0.85,
        changepoint_range=0.9  
    )
    
    debug_print("Fitting Prophet model (logistic growth with natural volatility)...")
    model.fit(df_train)
    save_model(model, item_id)

    tomorrow = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0) + timedelta(days=1)
    future_dates = [tomorrow + timedelta(days=i) for i in range(jours)]
    
    debug_print(f"Starting predictions from TOMORROW: {tomorrow}")
    debug_print(f"Last training date: {df_train['ds'].max()}")
    debug_print(f"Predicting {jours} days forward")
    
    future_df = pd.DataFrame({
        'ds': future_dates,
        'cap': [price_ceiling] * jours,
        'floor': [price_floor] * jours
    })
    
    forecast = model.predict(future_df)
    
    debug_print(f"First prediction (tomorrow): {forecast['yhat'].iloc[0]:.4f}€")
    debug_print(f"Last prediction ({jours} days ahead): {forecast['yhat'].iloc[-1]:.4f}€")
    
    debug_print("="*80)
    debug_print("FINAL PREDICTIONS (first 7 days):")
    debug_print(forecast[['ds', 'yhat', 'yhat_lower', 'yhat_upper']].head(7).to_string())
    debug_print("="*80)
    
    lstm_result = train_lstm_model(df_train, item_id)
    
    lstm_predictions = None
    if lstm_result[0] is not None and lstm_result[1] is not None:
        lstm_model, scaler, prices = lstm_result
        lookback = 7
        
        # Create price differences
        price_diffs = np.diff(prices)
        price_diffs = np.insert(price_diffs, 0, 0)
        
        prices_scaled = scaler.transform(price_diffs.reshape(-1, 1))
        last_sequence = prices_scaled[-lookback:].flatten()
        
        lstm_predictions = predict_lstm(lstm_model, scaler, last_sequence, prices, jours, lookback)
        
        debug_print("="*80)
        debug_print("LSTM PREDICTIONS (first 7 days):")
        for i in range(min(7, len(lstm_predictions))):
            debug_print(f"Day {i+1}: {lstm_predictions[i]:.4f}€")
        debug_print("="*80)
    
    if debug:
        draw_debug_graph(df_train, forecast, skin_id, wear_type_id, item_id, last_historical_date)
        if lstm_predictions is not None:
            draw_lstm_graph(df_train, lstm_predictions, future_dates, skin_id, wear_type_id, item_id, last_historical_date)

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

def draw_debug_graph(df_train: pd.DataFrame, forecast: pd.DataFrame, skin_id: int, 
                     wear_type_id: int, item_id: int, last_real_date: datetime) -> bool:
    lookback_days = 60
    cutoff_date = datetime.now() - timedelta(days=lookback_days)
    
    df_recent = df_train[df_train['ds'] >= cutoff_date].copy()
    
    df_real = df_recent[df_recent['ds'] <= last_real_date]
    df_gap = df_recent[df_recent['ds'] > last_real_date]
    
    fig, ax = plt.subplots(figsize=(16, 8), dpi=100)
    
    ax.plot(df_real['ds'], df_real['y'], 'o-', color='#1f77b4', label='Historical Data', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    if len(df_gap) > 0:
        last_real_price = df_real['y'].iloc[-1]
        last_real_date_val = df_real['ds'].iloc[-1]
        first_pred_date = forecast['ds'].iloc[0]
        first_pred_price = forecast['yhat'].iloc[0]
        
        ax.plot([last_real_date_val, first_pred_date], 
                [last_real_price, first_pred_price], 
                '--', color='gray', linewidth=1.5, alpha=0.5, label='Gap')
    
    ax.plot(forecast['ds'], forecast['yhat'], 'o-', color='#ff7f0e', label='Predictions', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    ax.fill_between(forecast['ds'], 
                     forecast['yhat_lower'], 
                     forecast['yhat_upper'], 
                     alpha=0.3, 
                     color='#ff7f0e', 
                     label='Confidence Interval')
    
    today = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
    ax.axvline(x=today, color='#2ca02c', linestyle='--', linewidth=2, alpha=0.7, label='Today')
    
    ax.set_xlabel('Date', fontsize=14, fontweight='bold')
    ax.set_ylabel('Price (€)', fontsize=14, fontweight='bold')
    
    title = f'Price Prediction - Skin ID: {skin_id}, Wear Type: {wear_type_id}\n'
    title += f'(Last {lookback_days} days historical + {len(forecast)} days forecast)\n'
    title += f'PROPHET model (logistic)'
    
    ax.set_title(title, fontsize=16, fontweight='bold')
    ax.legend(loc='best', fontsize=12, framealpha=0.95)
    ax.grid(True, alpha=0.3, linestyle='--')
    
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%d %b %Y'))
    
    total_days = (forecast['ds'].iloc[-1] - df_real['ds'].iloc[0]).days
    if total_days > 60:
        interval = 10
    elif total_days > 30:
        interval = 5
    else:
        interval = 3
    
    ax.xaxis.set_major_locator(mdates.DayLocator(interval=interval))
    plt.xticks(rotation=45, ha='right', fontsize=10)
    plt.yticks(fontsize=11)
    
    plt.tight_layout()
    
    graphs_dir = os.path.join(os.path.dirname(__file__), 'graphs')
    os.makedirs(graphs_dir, exist_ok=True)
    
    graph_path = os.path.join(graphs_dir, f'prediction_prophet_item_{item_id}_skin_{skin_id}_wear_{wear_type_id}.png')
    plt.savefig(graph_path, dpi=200, bbox_inches='tight')
    debug_print(f"Graph saved: {graph_path}")
    
    plt.close()
    return True

def draw_lstm_graph(df_train: pd.DataFrame, lstm_predictions: np.ndarray, future_dates: list,
                    skin_id: int, wear_type_id: int, item_id: int, last_real_date: datetime) -> bool:
    lookback_days = 60
    cutoff_date = datetime.now() - timedelta(days=lookback_days)
    
    df_recent = df_train[df_train['ds'] >= cutoff_date].copy()
    
    df_real = df_recent[df_recent['ds'] <= last_real_date]
    df_gap = df_recent[df_recent['ds'] > last_real_date]
    
    fig, ax = plt.subplots(figsize=(16, 8), dpi=100)
    
    ax.plot(df_real['ds'], df_real['y'], 'o-', color='#1f77b4', label='Historical Data', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    if len(df_gap) > 0:
        last_real_price = df_real['y'].iloc[-1]
        last_real_date_val = df_real['ds'].iloc[-1]
        first_pred_date = future_dates[0]
        first_pred_price = lstm_predictions[0]
        
        ax.plot([last_real_date_val, first_pred_date], 
                [last_real_price, first_pred_price], 
                '--', color='gray', linewidth=1.5, alpha=0.5, label='Gap')
    
    ax.plot(future_dates, lstm_predictions, 'o-', color='#d62728', label='LSTM Predictions', 
            linewidth=2.5, markersize=6, alpha=0.8)
    
    lstm_std = np.std(lstm_predictions) * 0.5
    lower_bound = lstm_predictions - lstm_std
    upper_bound = lstm_predictions + lstm_std
    
    ax.fill_between(future_dates, lower_bound, upper_bound, alpha=0.3, color='#d62728', 
                     label='Confidence Interval')
    
    today = datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)
    ax.axvline(x=today, color='#2ca02c', linestyle='--', linewidth=2, alpha=0.7, label='Today')
    
    ax.set_xlabel('Date', fontsize=14, fontweight='bold')
    ax.set_ylabel('Price (€)', fontsize=14, fontweight='bold')
    
    title = f'Price Prediction - Skin ID: {skin_id}, Wear Type: {wear_type_id}\n'
    title += f'(Last {lookback_days} days historical + {len(lstm_predictions)} days forecast)\n'
    title += f'LSTM model (deep learning)'
    
    ax.set_title(title, fontsize=16, fontweight='bold')
    ax.legend(loc='best', fontsize=12, framealpha=0.95)
    ax.grid(True, alpha=0.3, linestyle='--')
    
    ax.xaxis.set_major_formatter(mdates.DateFormatter('%d %b %Y'))
    
    total_days = (future_dates[-1] - df_real['ds'].iloc[0]).days
    if total_days > 60:
        interval = 10
    elif total_days > 30:
        interval = 5
    else:
        interval = 3
    
    ax.xaxis.set_major_locator(mdates.DayLocator(interval=interval))
    plt.xticks(rotation=45, ha='right', fontsize=10)
    plt.yticks(fontsize=11)
    
    plt.tight_layout()
    
    graphs_dir = os.path.join(os.path.dirname(__file__), 'graphs')
    os.makedirs(graphs_dir, exist_ok=True)
    
    graph_path = os.path.join(graphs_dir, f'prediction_lstm_item_{item_id}_skin_{skin_id}_wear_{wear_type_id}.png')
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

