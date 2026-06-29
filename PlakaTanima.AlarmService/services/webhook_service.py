import requests
import base64
import config

def to_base64(image_bytes):
    if not image_bytes: return None
    return base64.b64encode(image_bytes).decode('utf-8')

def send_webhook(event):
    # Sends a lightweight notification webhook to ASP.NET to alert it of a new event in the DB.
    payload = {
        "eventId": event.event_id,
        "plate": event.plate,
        "cameraName": event.camera_name,
        "timestamp": event.timestamp,
        "hasImages": True
    }

    try:
        # Send to ASP.NET Webhook
        response = requests.post(config.WEBHOOK_URL, json=payload, timeout=config.WEBHOOK_TIMEOUT)
        response.raise_for_status()
    except Exception as e:
        print(f"Webhook notification failed: {e}")
