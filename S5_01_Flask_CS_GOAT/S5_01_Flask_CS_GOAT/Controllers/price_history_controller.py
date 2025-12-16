from flask import Blueprint, jsonify, request
from datetime import datetime
from ..services.model import PriceHistory, Wear, db
from ..fonctionnalites.ia_app import predict_and_save

price_history_bp = Blueprint('price_history', __name__, url_prefix='/api/price_history')


@price_history_bp.route('/predict_price/<int:skin_id>/<int:wear_id>', methods=['GET'])
@price_history_bp.route('/predict_price/<int:skin_id>/<int:wear_id>/<int:jours>', methods=['GET'])
def predict_price(skin_id, wear_id, jours=30):
        
        if(predict_and_save(skin_id, wear_id, jours)) is None:
            return jsonify({'message': 'Not enough data to make a prediction'}), 400
        return jsonify({'message': 'prediction created'}), 200
