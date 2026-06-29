import io
import json
from pathlib import Path
from datetime import datetime
from PIL import Image
import config
from services.minio_service import upload_to_minio

def ensure_camera_dirs(camera_name):
    base = config.BASE_FOLDER / camera_name
    base.mkdir(parents=True, exist_ok=True)

def save_event_files(event):
    """
    Saves event XML/JSON metadata and converts JPG images to PNG,
    saving them locally under the hierarchical directory structure,
    and uploading them to MinIO.
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

    # Save metadata as JSON locally
    json_str = json.dumps(event.xml_data, indent=4, ensure_ascii=False)
    json_path = dest_dir / "event.json"
    with open(json_path, "w", encoding="utf-8") as f:
        f.write(json_str)

    # Upload JSON metadata to MinIO
    json_bytes = json_str.encode("utf-8")
    json_object_key = f"{event.camera_name}/{year}/{month}/{day}/{hour_range}/{folder_name}/event.json"
    upload_to_minio(json_object_key, json_bytes, "application/json")

    saved_paths = {}

    # Save images locally & upload to MinIO
    for image_type, image_data in event.images.items():
        png_bytes = None
        content_type = "image/png"
        file_ext = "png"

        try:
            img = Image.open(io.BytesIO(image_data))
            out_buf = io.BytesIO()
            img.save(out_buf, format="PNG")
            png_bytes = out_buf.getvalue()
        except Exception as e:
            print(f"Pillow conversion failed for {image_type}, using raw JPEG: {e}")
            png_bytes = image_data
            content_type = "image/jpeg"
            file_ext = "jpg"

        # Save locally as backup
        file_name = f"{image_type}.{file_ext}"
        file_path = dest_dir / file_name
        with open(file_path, "wb") as f:
            f.write(png_bytes)

        # Upload to MinIO
        object_key = f"{event.camera_name}/{year}/{month}/{day}/{hour_range}/{folder_name}/{file_name}"
        minio_path = upload_to_minio(object_key, png_bytes, content_type)

        if minio_path:
            saved_paths[image_type] = minio_path
        else:
            # Fallback to relative local path if upload fails
            relative_path = file_path.relative_to(config.BASE_FOLDER)
            saved_paths[image_type] = str(relative_path)

    return saved_paths
