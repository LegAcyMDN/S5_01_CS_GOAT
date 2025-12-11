from flask import Blueprint, jsonify
from ..services.model import PriceHistory, Wear

price_history_bp = Blueprint('price_history', __name__, url_prefix='/api/PriceHistory')


@price_history_bp.route('/{wear_id}', methods=['GET'])
def get_by_id(wear_id):
    
    wear = Wear.query.get_or_404(wear_id)
    return get_all(wear.skin_id, wear.wear_type_id)


@price_history_bp.route('/{skin_id}/{wear_type_id}', methods=['GET'])
def get_all(skin_id, wear_type_id):
    price_histories = PriceHistory.query.filter_by(
        skin_id=skin_id,
        wear_type_id=wear_type_id
    ).all()
    
    if not price_histories:
        return jsonify({'error': 'Not found'}), 404
    
    return jsonify([ph.to_dict() for ph in price_histories]), 200