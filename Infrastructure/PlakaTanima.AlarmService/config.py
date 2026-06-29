import os
from pathlib import Path

# Base folder where images are saved locally
BASE_FOLDER = Path(os.getenv("BASE_FOLDER", "Plaka"))

# PostgreSQL connection details
DB_HOST = os.getenv("DB_HOST", "localhost")
DB_PORT = os.getenv("DB_PORT", "5432")
DB_NAME = os.getenv("DB_NAME", "platar-test-db")
DB_USER = os.getenv("DB_USER", "postgres")
DB_PASS = os.getenv("DB_PASS", "admin123")

# Webhook destination (kept for backward compatibility or notification trigger)
WEBHOOK_URL = os.getenv("WEBHOOK_URL", "http://localhost:5233/api/lpr/event")
WEBHOOK_TIMEOUT = int(os.getenv("WEBHOOK_TIMEOUT", "10"))

RECONNECT_DELAY = int(os.getenv("RECONNECT_DELAY", "5"))
STREAM_TIMEOUT = int(os.getenv("STREAM_TIMEOUT", "60"))

FOLDERS = {
    "xml": "Xml",
    "json": "VehicleDetails",
    "plate": "VehiclePlate",
    "vehicle": "VehicleImage",
    "full": "VehicleFullImage",
    "unknown": "UnknownImages"
}

# Camera streaming list
KAMERALAR = [
    {
        "ad": "30Agustos_GIRIS",
        "ip": "10.179.0.81",
        "user": "admin",
        "pass": "kutup12."
    },
    {
        "ad": "30Agustos_CIKIS",
        "ip": "10.179.0.80",
        "user": "admin",
        "pass": "kutup12."
    }
]
