import time
import threading
from multiprocessing import Process, Queue
import config

from camera.camera_process import CameraProcess
from workers.storage_worker import storage_worker
from workers.webhook_worker import webhook_worker
from utils.logger import logger
from services.db_service import get_all_cameras

def run_camera_listener(listener):
    listener.start()

def config_changed(cfg1, cfg2):
    return (
        cfg1.get("ip") != cfg2.get("ip") or
        cfg1.get("user") != cfg2.get("user") or
        cfg1.get("pass") != cfg2.get("pass") or
        cfg1.get("ad") != cfg2.get("ad")
    )

if __name__ == "__main__":
    logger.info("Starting Platar High-Performance Alarm Service...")

    storage_queue = Queue()
    webhook_queue = Queue()

    # 1. Start parallel worker subprocesses to consume events concurrently
    storage_processes = []
    for i in range(3):
        p = Process(target=storage_worker, args=(storage_queue,), name=f"StorageWorker-{i}")
        p.daemon = True
        p.start()
        storage_processes.append(p)
    logger.info(f"Spawned 3 parallel storage worker processes.")

    webhook_processes = []
    for i in range(2):
        p = Process(target=webhook_worker, args=(webhook_queue,), name=f"WebhookWorker-{i}")
        p.daemon = True
        p.start()
        webhook_processes.append(p)
    logger.info(f"Spawned 2 parallel webhook worker processes.")

    running_cameras = {}  # {camera_id: {"config": cam_cfg, "listener": cam_listener, "thread": thread}}

    try:
        while True:
            # Poll camera metadata from PostgreSQL database
            db_cameras = get_all_cameras()
            db_camera_ids = {cam["id"] for cam in db_cameras}

            # 2. Terminate and cleanup removed camera listener threads
            to_remove = []
            for cam_id, run_data in running_cameras.items():
                if cam_id not in db_camera_ids:
                    logger.info(f"Camera removed from DB: {run_data['config']['ad']}. Stopping thread...")
                    run_data["listener"].stop()
                    run_data["thread"].join(timeout=1.0)
                    to_remove.append(cam_id)
            for cam_id in to_remove:
                del running_cameras[cam_id]

            # 3. Add new or restart updated camera listener threads
            for cam in db_cameras:
                cam_id = cam["id"]
                if cam_id not in running_cameras:
                    logger.info(f"New camera found in DB: {cam['ad']} ({cam['ip']}). Spawning listener thread...")
                    cam_listener = CameraProcess(cam, storage_queue, webhook_queue)
                    t = threading.Thread(target=run_camera_listener, args=(cam_listener,), name=f"CamListener-{cam['ad']}")
                    t.daemon = True
                    t.start()
                    running_cameras[cam_id] = {
                        "config": cam,
                        "listener": cam_listener,
                        "thread": t
                    }
                else:
                    # Check if connection parameters changed
                    current_cfg = running_cameras[cam_id]["config"]
                    if config_changed(current_cfg, cam):
                        logger.info(f"Config changed for camera: {cam['ad']}. Restarting listener thread...")
                        running_cameras[cam_id]["listener"].stop()
                        running_cameras[cam_id]["thread"].join(timeout=1.0)

                        cam_listener = CameraProcess(cam, storage_queue, webhook_queue)
                        t = threading.Thread(target=run_camera_listener, args=(cam_listener,), name=f"CamListener-{cam['ad']}")
                        t.daemon = True
                        t.start()
                        running_cameras[cam_id] = {
                            "config": cam,
                            "listener": cam_listener,
                            "thread": t
                        }

            time.sleep(5)

    except KeyboardInterrupt:
        logger.info("Stopping Platar Alarm Service...")
        
        # Stop camera listeners
        for cam_id, run_data in running_cameras.items():
            run_data["listener"].stop()
            run_data["thread"].join(timeout=1.0)
            
        # Terminate worker processes
        for p in storage_processes:
            p.terminate()
        for p in webhook_processes:
            p.terminate()
            
        logger.info("Platar Alarm Service stopped cleanly.")
