import psycopg2
from datetime import datetime, timezone
import config
from utils.logger import logger

def get_connection():
    return psycopg2.connect(
        host=config.DB_HOST,
        port=config.DB_PORT,
        database=config.DB_NAME,
        user=config.DB_USER,
        password=config.DB_PASS
    )

def save_event_to_db(event, saved_image_paths):
    """
    Inserts a new ANPR Event record directly into PostgreSQL.
    Supports robust parsing for different XML tag formats (case sensitivity, casing variation).
    """
    anpr = event.xml_data.get("ANPR", {})
    vehicle_info = anpr.get("vehicleInfo") or anpr.get("VehicleInfo") or {}

    # Extract details safely
    confidence_str = anpr.get("confidenceLevel") or anpr.get("ConfidenceLevel", "95")
    confidence_str = str(confidence_str).replace(",", ".")
    try:
        confidence = float(confidence_str) / 100
    except ValueError:
        confidence = 0.95

    vehicle_type = (
        anpr.get("vehicleType") or
        anpr.get("VehicleType") or
        vehicle_info.get("vehicleType") or
        vehicle_info.get("VehicleType") or
        "Unknown"
    )

    vehicle_color = (
        vehicle_info.get("color") or
        vehicle_info.get("Color") or
        "Unknown"
    )

    vehicle_brand = (
        vehicle_info.get("brand") or
        vehicle_info.get("Brand") or
        vehicle_info.get("vehicleLogoRecog") or
        "Unknown"
    )

    direction = (
        anpr.get("direction") or
        anpr.get("Direction") or
        anpr.get("movingDirection") or
        "Unknown"
    )

    country = (
        anpr.get("country") or
        anpr.get("Country") or
        "Turkey"
    )

    # Image paths
    plate_path = saved_image_paths.get("plate")
    vehicle_path = saved_image_paths.get("vehicle")
    full_path = saved_image_paths.get("full")

    # Try parsing XML dateTime
    dt = None
    xml_dt_str = event.xml_data.get("dateTime")
    if xml_dt_str:
        try:
            dt = datetime.fromisoformat(xml_dt_str)
        except Exception:
            pass

    if not dt:
        try:
            dt = datetime.strptime(event.timestamp, "%Y%m%d_%H%M%S_%f")
            dt = dt.replace(tzinfo=timezone.utc)
        except Exception:
            dt = datetime.now(timezone.utc)

    now = datetime.now(timezone.utc)

    query = """
        INSERT INTO "AnprEvents" (
            "Id", "Plate", "CameraName", "EventTimestamp", "Confidence",
            "VehicleType", "VehicleColor", "VehicleBrand", "Direction", "Country",
            "PlateImagePath", "VehicleImagePath", "FullImagePath", "CreatedAt"
        ) VALUES (%s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s, %s)
    """

    conn = None
    try:
        conn = get_connection()
        with conn.cursor() as cur:
            cur.execute(query, (
                event.event_id,
                event.plate,
                event.camera_name,
                dt,
                confidence,
                vehicle_type,
                vehicle_color,
                vehicle_brand,
                direction,
                country,
                plate_path,
                vehicle_path,
                full_path,
                now
            ))
            conn.commit()
            logger.info(f"[DB] Successfully inserted ANPR event for plate {event.plate} with ID {event.event_id}")
    except Exception as e:
        logger.error(f"[DB] Error executing PostgreSQL insert query: {e}")
        if conn:
            conn.rollback()
        raise e
    finally:
        if conn:
            conn.close()
