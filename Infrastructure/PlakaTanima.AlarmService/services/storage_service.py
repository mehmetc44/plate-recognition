import io
import json
from pathlib import Path
from datetime import datetime
from PIL import Image
import config

def ensure_camera_dirs(camera_name):
    base = config.BASE_FOLDER / camera_name
    base.mkdir(parents=True, exist_ok=True)

def save_event_files(event):
    """
    Saves event XML/JSON metadata and converts JPG images to PNG,
    saving them under the hierarchical directory structure:
    BASE_FOLDER/{camera_name}/{year}/{month}/{day}/{hour_range}/{plate}_{timestamp}/
    """
    # Try parsing XML dateTime first
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
        except Exception:
            dt = datetime.now()

    year = dt.strftime("%Y")
    month = dt.strftime("%m")
    day = dt.strftime("%d")

    # Hour range: e.g., 15.00-16.00
    hour = dt.hour
    next_hour = (hour + 1) % 24
    hour_range = f"{hour:02d}.00-{next_hour:02d}.00"

    # Directory name: plate_datehourminute
    folder_name = f"{event.plate}_{dt.strftime('%Y%m%d_%H%M%S')}"

    # Build full destination path
    dest_dir = config.BASE_FOLDER / event.camera_name / year / month / day / hour_range / folder_name
    dest_dir.mkdir(parents=True, exist_ok=True)

    # Save metadata as JSON
    json_path = dest_dir / "event.json"
    with open(json_path, "w", encoding="utf-8") as f:
        json.dump(event.xml_data, f, indent=4, ensure_ascii=False)

    saved_paths = {}

    # Save images converting JPEG to PNG
    for image_type, image_data in event.images.items():
        file_name = f"{image_type}.png"
        file_path = dest_dir / file_name

        try:
            img = Image.open(io.BytesIO(image_data))
            img.save(file_path, format="PNG")
        except Exception as e:
            print(f"Pillow conversion failed for {image_type}, saving raw: {e}")
            fallback_path = dest_dir / f"{image_type}.jpg"
            with open(fallback_path, "wb") as f:
                f.write(image_data)
            file_path = fallback_path

        relative_path = file_path.relative_to(config.BASE_FOLDER)
        saved_paths[image_type] = str(relative_path)

    return saved_paths
