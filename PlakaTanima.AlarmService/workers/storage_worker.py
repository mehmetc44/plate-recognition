from services.storage_service import save_event_files
from services.db_service import save_event_to_db
from utils.logger import logger

def storage_worker(storage_queue):
    logger.info("[STORAGE] Storage & DB Worker started")
    while True:
        try:
            event = storage_queue.get()
            logger.info(f"[STORAGE] Processing event for plate: {event.plate}")

            # 1. Save images locally hierarchically and convert them to PNG
            saved_paths = save_event_files(event)
            logger.info(f"[STORAGE] Successfully saved local files. Plate: {event.plate}")

            # 2. Insert event record directly into PostgreSQL
            save_event_to_db(event, saved_paths)
            
        except Exception as e:
            logger.error(f"[STORAGE] Error processing storage or DB save: {e}")
