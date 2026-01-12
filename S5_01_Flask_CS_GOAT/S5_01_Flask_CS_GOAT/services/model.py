from flask_sqlalchemy import SQLAlchemy
from datetime import datetime

db = SQLAlchemy()

class Skin(db.Model):
    __tablename__ = 't_e_skin_skn'
    
    skin_id = db.Column('skn_id', db.Integer, primary_key=True, autoincrement=True)
    skin_name = db.Column('skn_skinname', db.String(255), nullable=False, index=True)
    paint_index = db.Column('skn_paintindex', db.Integer, nullable=False, index=True)
    uv_type = db.Column('skn_uvtype', db.Integer, nullable=False)
    item_id = db.Column('itm_id', db.Integer, nullable=False)
    rarity_id = db.Column('rar_id', db.Integer, nullable=False)
    
    def to_dict(self):
        return {
            'skinid': self.skin_id,
            'skinname': self.skin_name,
            'paintindex': self.paint_index,
            'uvtype': self.uv_type,
            'itemid': self.item_id,
            'rarityid': self.rarity_id
        }
    
    def __str__(self):
        return str(self.to_dict())

class Wear(db.Model):
    __tablename__ = 't_e_wear_wer'
    
    wear_id = db.Column('wer_id', db.Integer, primary_key=True, autoincrement=True)
    wear_type_id = db.Column('wrt_id', db.Integer, nullable=False)
    skin_id = db.Column('skn_id', db.Integer, nullable=False)

class PriceHistory(db.Model):
    __tablename__ = 't_e_pricehistory_prh'
    
    price_history_id = db.Column('prh_id', db.Integer, primary_key=True, autoincrement=True)
    price_date = db.Column('prh_pricedate', db.DateTime, nullable=False, index=True)
    price_value = db.Column('prh_pricevalue', db.Float, nullable=False)
    guess_date = db.Column('prh_guessdate', db.DateTime, nullable=True) 
    volume = db.Column('prh_volume', db.Integer, nullable=True)
    
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
    def __str__(self):
        return str(self.to_dict())

class Item(db.Model):
    __tablename__ = 't_e_item_itm'

    item_id = db.Column('itm_id', db.Integer, primary_key=True, autoincrement=True)
    item_name = db.Column('itm_itemname', db.String(255), nullable=False, index=True)
    item_model = db.Column('itm_itemmodel', db.String(255), nullable=False)
    def_index = db.Column('itm_defindex', db.Integer, nullable=True)
    item_type_id = db.Column('itt_id', db.Integer, nullable=False)

    def to_dict(self):
        return {
            'itemid': self.item_id,
            'itemname': self.item_name,
            'itemmodel': self.item_model,
            'defindex': self.def_index,
            'itemtypeid': self.item_type_id
        }

    def __str__(self):
        return str(self.to_dict())

class WearType(db.Model):
    __tablename__ = 't_e_weartype_wrt'

    wear_type_id = db.Column('wrt_id', db.Integer, primary_key=True, autoincrement=True)
    wear_type_name = db.Column('wrt_weartypename', db.String(100), nullable=False, unique=True)

    def to_dict(self):
        return {
            'weartypeid': self.wear_type_id,
            'weartypename': self.wear_type_name
        }

    def __str__(self):
        return str(self.to_dict())