""" The flask application package. """
from flask import Flask
from flask_cors import CORS
from dotenv import load_dotenv
import os

# Load environment variables first
load_dotenv()

from .services.model import db


basedir = os.path.abspath(os.path.dirname(__file__))

# Initialize Flask app
app = Flask(__name__)

# Configure CORS to allow requests from Blazor
CORS(app, resources={
    r"/api/*": {
        "origins": [
            "https://localhost:7030",  # Blazor HTTPS en développement
            "http://localhost:5028",   # Blazor HTTP en développement
            "https://localhost:44328", # Blazor IIS Express
            "http://localhost:50491",  # Blazor IIS Express HTTP
            "https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net",  # Production
        ],
        "methods": ["GET", "POST", "PUT", "DELETE", "OPTIONS"],
        "allow_headers": ["Content-Type", "Authorization"],
        "supports_credentials": True
    }
})

# Configure the app for PostgreSQL
app.config['SQLALCHEMY_DATABASE_URI'] = os.getenv("SQLALCHEMY_DATABASE_URI")
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
app.config['DEBUG_MODE'] = os.getenv("DEBUG_MODE", "False").lower() == "true"
# Initialize SQLAlchemy with the app
db.init_app(app)

# Configure debug mode
debug = app.config['DEBUG_MODE']
print(f" * Debug print is {'on!' if debug  else 'off!'}")

def debug_print(msg: str) -> None:
    if debug:
        print(msg)

from .Controllers.price_history_controller import price_history_bp
# Register blueprints
app.register_blueprint(price_history_bp)