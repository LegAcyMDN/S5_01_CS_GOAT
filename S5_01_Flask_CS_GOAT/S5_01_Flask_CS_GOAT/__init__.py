""" The flask application package. """
from flask import Flask
from dotenv import load_dotenv
import os

# Load environment variables first
load_dotenv()

from .services.model import db
from .Controllers.price_history_controller import price_history_bp

basedir = os.path.abspath(os.path.dirname(__file__))

# Initialize Flask app
app = Flask(__name__)

# Configure the app for PostgreSQL
app.config['SQLALCHEMY_DATABASE_URI'] = os.getenv("SQLALCHEMY_DATABASE_URI")
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

# Initialize SQLAlchemy with the app
db.init_app(app)

# Register blueprints
app.register_blueprint(price_history_bp)