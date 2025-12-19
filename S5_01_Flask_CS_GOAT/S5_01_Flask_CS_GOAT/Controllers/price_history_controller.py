from flask import Blueprint, jsonify, request
from datetime import datetime
from ..services.model import PriceHistory, Wear, db
from ..fonctionnalites.ia_app import predict_and_save


price_history_bp = Blueprint('price_history', __name__, url_prefix='/api/price_history')



@price_history_bp.route('/predict_price/bywear/<int:wear_id>', methods=['GET'])
@price_history_bp.route('/predict_price/bywear/<int:wear_id>/<int:jours>', methods=['GET'])
def predict_price(wear_id:int, jours:int=30):
    if predict_and_save(jours, wear_id) is None:
        return jsonify({'message': 'Wear not found'}), 404
    return jsonify({'message': 'prediction created'}), 200



@price_history_bp.route('/predict_price/byall/<int:skin_id>/<int:weartype_id>', methods=['GET'])
@price_history_bp.route('/predict_price/byall/<int:skin_id>/<int:weartype_id>/<int:jours>', methods=['GET'])
def predict_price_weartype(skin_id:int, weartype_id:int, jours:int=30):    
    if predict_and_save(jours,None,skin_id,weartype_id) is None:
        return jsonify({'message': 'Wear not found'}), 404
    return jsonify({'message': 'prediction created'}), 200