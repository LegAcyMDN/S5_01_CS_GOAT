from flask_sqlalchemy import SQLAlchemy

db = SQLAlchemy()

class Wear(db.Model):
    """Table Wear"""
    __tablename__ = 't_e_wear_wer'
    
    wear_id = db.Column('wer_id', db.Integer, primary_key=True, autoincrement=True)
    wear_type_id = db.Column('wrt_id', db.Integer, nullable=False)
    skin_id = db.Column('skn_id', db.Integer, nullable=False)

class PriceHistory(db.Model):
    """Table pour l'historique des prix"""
    __tablename__ = 't_e_pricehistory_prh'
    
    price_history_id = db.Column('prh_id', db.Integer, primary_key=True, autoincrement=True)
    price_date = db.Column('prh_pricedate', db.DateTime, nullable=False, index=True)
    price_value = db.Column('prh_pricevalue', db.Float, nullable=False)
    
    skin_id = db.Column('skn_id', db.Integer, nullable=False)
    wear_type_id = db.Column('wrt_id', db.Integer, nullable=False)
    
    def to_dict(self):
        return {
            'pricehistoryid': self.price_history_id,
            'weartypeid': self.wear_type_id,
            'skinid': self.skin_id,
            'pricedate': self.price_date.isoformat(),
            'pricevalue': self.price_value
        }