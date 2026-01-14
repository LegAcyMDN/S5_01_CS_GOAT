from flask import Blueprint, jsonify, request, send_from_directory
from datetime import datetime
import os
from ..services.model import PriceHistory, Wear, db
from ..fonctionnalites.ia_app import predict_and_save


price_history_bp = Blueprint('price_history', __name__, url_prefix='/api/price_history')



@price_history_bp.route('/predict_price/bywear/<int:wear_id>', methods=['GET'])
@price_history_bp.route('/predict_price/bywear/<int:wear_id>/<int:jours>', methods=['GET'])
def predict_price(wear_id: int, jours: int = 30):
    result = predict_and_save(jours, wear_id)
    
    if result is None:
        return jsonify({'message': 'Wear not found'}), 404
    elif result == False:
        return jsonify({'message': 'Not enough data to train the model'}), 400
    
    # Retourner les chemins des graphiques si disponibles
    if isinstance(result, dict):
        return jsonify({
            'message': 'Prediction created',
            'graphs': result
        }), 200
    
    return jsonify({'message': 'Prediction created'}), 200


@price_history_bp.route('/predict_price/byall/<int:skin_id>/<int:weartype_id>', methods=['GET'])
@price_history_bp.route('/predict_price/byall/<int:skin_id>/<int:weartype_id>/<int:jours>', methods=['GET'])
def predict_price_weartype(skin_id: int, weartype_id: int, jours: int = 30):
    result = predict_and_save(jours, None, skin_id, weartype_id)
    
    if result is None:
        return jsonify({'message': 'Wear not found'}), 404
    elif result == False:
        return jsonify({'message': 'Not enough data to train the model'}), 400
    
    # Retourner les chemins des graphiques si disponibles
    if isinstance(result, dict):
        return jsonify({
            'message': 'Prediction created',
            'graphs': result
        }), 200
    
    return jsonify({'message': 'Prediction created'}), 200


@price_history_bp.route('/graphs/<path:filename>', methods=['GET'])
def get_graph(filename):
    """Endpoint pour servir les graphiques générés"""
    graphs_dir = os.path.join(os.path.dirname(__file__), '..', 'fonctionnalites', 'graphs')
    return send_from_directory(graphs_dir, filename)