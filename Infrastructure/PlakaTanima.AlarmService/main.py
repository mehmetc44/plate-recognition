import time
from multiprocessing import Process, Queue
import config

from camera.camera_process import CameraProcess
from workers.storage_worker import storage_worker
from workers.webhook_worker import webhook_worker
from utils.logger import logger
from services.db_service import get_all_cameras

def run_camera(cam_cfg, storage_queue, webhook_queue):
    cam = CameraProcess(cam_cfg, storage_queue, webhook_queue)
    cam.start()

def config_changed(cfg1, cfg2):
    return (
        cfg1.get("ip") != cfg2.get("ip") or
        cfg1.get("user") != cfg2.get("user") or
        cfg1.get("pass") != cfg2.get("pass") or
        cfg1.get("ad") != cfg2.get("ad")
    )

if __name__ == "__main__":
    logger.info("Starting Platar Alarm Service...")

    storage_queue = Queue()
    webhook_queue = Queue()

    # Start the workers
    storage_process = Process(target=storage_worker, args=(storage_queue,))
    webhook_process = Process(target=webhook_worker, args=(webhook_queue,))

    storage_process.start()
    webhook_process.start()

    running_cameras = {}  # {camera_id: {"config": cam_cfg, "process": process}}

    try:
        while True:
            # Poll cameras from database
            db_cameras = get_all_cameras()
            db_camera_ids = {cam["id"] for cam in db_cameras}

            # 1. Clean up deleted cameras
            to_remove = []
            for cam_id, run_data in running_cameras.items():
                if cam_id not in db_camera_ids:
                    logger.info(f"Camera removed from DB: {run_data['config']['ad']}. Terminating process...")
                    run_data["process"].terminate()
                    run_data["process"].join()
                    to_remove.append(cam_id)
            for cam_id in to_remove:
                del running_cameras[cam_id]

            # 2. Add or update cameras
            for cam in db_cameras:
                cam_id = cam["id"]
                if cam_id not in running_cameras:
                    logger.info(f"New camera found in DB: {cam['ad']} ({cam['ip']}). Spawning listener process...")
                    p = Process(target=run_camera, args=(cam, storage_queue, webhook_queue))
                    p.start()
                    running_cameras[cam_id] = {"config": cam, "process": p}
                else:
                    # Check if config changed
                    current_cfg = running_cameras[cam_id]["config"]
                    if config_changed(current_cfg, cam):
                        logger.info(f"Config changed for camera: {cam['ad']}. Restarting listener process...")
                        running_cameras[cam_id]["process"].terminate()
                        running_cameras[cam_id]["process"].join()

                        p = Process(target=run_camera, args=(cam, storage_queue, webhook_queue))
                        p.start()
                        running_cameras[cam_id] = {"config": cam, "process": p}

            time.sleep(5)
    except KeyboardInterrupt:
        logger.info("Stopping Platar Alarm Service...")
        storage_process.terminate()
        webhook_process.terminate()
        for cam_id, run_data in running_cameras.items():
            run_data["process"].terminate()
            run_data["process"].join()
        logger.info("Platar Alarm Service stopped.")
