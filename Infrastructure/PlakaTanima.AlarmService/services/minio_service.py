from minio import Minio
import io
import config
from utils.logger import logger

_client = None

def get_minio_client():
    global _client
    if _client is None:
        try:
            _client = Minio(
                config.MINIO_ENDPOINT,
                access_key=config.MINIO_ACCESS_KEY,
                secret_key=config.MINIO_SECRET_KEY,
                secure=config.MINIO_SECURE
            )
            # Ensure bucket exists
            if not _client.bucket_exists(config.MINIO_BUCKET):
                _client.make_bucket(config.MINIO_BUCKET)
                logger.info(f"[MINIO] Successfully created bucket: {config.MINIO_BUCKET}")
        except Exception as e:
            logger.error(f"[MINIO] Error initializing client connection: {e}")
            raise e
    return _client

def upload_to_minio(object_name, data_bytes, content_type="application/octet-stream"):
    """
    Uploads raw bytes to the MinIO bucket.
    Returns the mapped location identifier string if successful.
    """
    try:
        client = get_minio_client()
        stream = io.BytesIO(data_bytes)
        client.put_object(
            config.MINIO_BUCKET,
            object_name,
            stream,
            length=len(data_bytes),
            content_type=content_type
        )
        logger.info(f"[MINIO] Uploaded: {config.MINIO_BUCKET}/{object_name}")
        return f"{config.MINIO_BUCKET}/{object_name}"
    except Exception as e:
        logger.error(f"[MINIO] Failed to upload object {object_name}: {e}")
        return None
