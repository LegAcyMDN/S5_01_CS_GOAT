"""
The flask application package.
"""

from flask import Flask
from flask_sqlalchemy import SQLAlchemy
from dotenv import load_dotenv
import os
from services.models import db

basedir = os.path.abspath(os.path.dirname(__file__))

# Initialize Flask app
app = Flask(__name__)


# Load environment variables
load_dotenv()


db.init_app(app)

# Configure the app for PostgreSQL
app.config['SQLALCHEMY_DATABASE_URI'] = os.getenv("POSTGRESQL_DATABASE_URI")
app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False

# Initialize SQLAlchemy
db = SQLAlchemy(app)
