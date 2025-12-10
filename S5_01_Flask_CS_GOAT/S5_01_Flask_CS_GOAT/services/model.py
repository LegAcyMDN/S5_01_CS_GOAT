from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()

class PriceHistory(db.Model):
    """Table pour l'historique des prix"""
    __tablename__ = 't_e_pricehistory_prh'
    
    price_history_id = db.Column('prh_id', db.Integer, primary_key=True, autoincrement=True)
    price_date = db.Column('prh_pricedate', db.DateTime, nullable=False, index=True)
    price_value = db.Column('prh_pricevalue', db.Float, nullable=False)
    
    # Clés étrangères
    skin_id = db.Column('skn_id', db.Integer, db.ForeignKey('t_e_skin_skn.skn_id'), nullable=False)
    wear_type_id = db.Column('wrt_id', db.Integer, db.ForeignKey('t_e_weartype_wrt.wrt_id'), nullable=False)
    
    # Relations
    skin = db.relationship('Skin', back_populates='price_histories')
    wear_type = db.relationship('WearType', back_populates='price_histories')
    
    # Contrainte unique pour éviter les doublons
    __table_args__ = (
        db.UniqueConstraint('skn_id', 'wrt_id', 'prh_pricedate', name='uq_skin_wear_date'),
        db.Index('idx_date_price', 'prh_pricedate', 'prh_pricevalue'),
    )
    
    def __repr__(self):
        return f'<PriceHistory {self.price_date} - ${self.price_value}>'
    
    def to_dict(self):
        """Convertit l'objet en dictionnaire pour l'API"""
        return {
            'id': self.price_history_id,
            'date': self.price_date.isoformat(),
            'price': self.price_value,
            'skin': {
                'id': self.skin.skin_id,
                'name': self.skin.skin_name,
                'item': self.skin.item.item_name
            },
            'wear': self.wear_type.wear_type_name
        }
