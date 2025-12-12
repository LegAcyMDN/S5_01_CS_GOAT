from flask import Blueprint, jsonify,request
from datetime import datetime
from ..services.model import PriceHistory, Wear,db

price_history_bp = Blueprint('price_history', __name__, url_prefix='/api/PriceHistory')



@price_history_bp.route('/add', methods=['POST'])
def create_prediction():

    data = request.get_json()
    
    new_price = PriceHistory(
        skin_id=data['skinid'],
        wear_type_id=data['weartypeid'],
        price_value=data['pricevalue'],
        price_date=datetime.fromisoformat(data['pricedate']),
        guess_date=datetime.now()
    )
    
    db.session.add(new_price)
    db.session.commit()
    
    return jsonify(new_price.to_dict()), 201


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

@price_history_bp.route('/<int:price_history_id>', methods=['PUT'])
def update_guess_date(price_history_id):
    """Update guess date after AI prediction"""
    price_history = PriceHistory.query.get_or_404(price_history_id)
    
    price_history.guess_date = datetime.now()
    db.session.commit()
    
    return jsonify(price_history.to_dict()), 200