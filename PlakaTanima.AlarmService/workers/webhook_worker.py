from services.webhook_service import send_webhook
from utils.logger import logger

def webhook_worker(webhook_queue):
    logger.info("[WEBHOOK] Notification Worker started")
    while True:
        try:
            event = webhook_queue.get()
            send_webhook(event)
            logger.info(f"[WEBHOOK] Notification sent for plate: {event.plate}")
        except Exception as e:
            logger.error(f"[WEBHOOK] Error sending notification: {e}")
