from multiprocessing import Process, Queue
import config

from camera.camera_process import CameraProcess
from workers.storage_worker import storage_worker
from workers.webhook_worker import webhook_worker
from utils.logger import logger

def run_camera(cam_cfg, storage_queue, webhook_queue):
    cam = CameraProcess(cam_cfg, storage_queue, webhook_queue)
    cam.start()

if __name__ == "__main__":
    logger.info("Starting Platar Alarm Service...")

    storage_queue = Queue()
    webhook_queue = Queue()

    # Start the workers
    storage_process = Process(target=storage_worker, args=(storage_queue,))
    webhook_process = Process(target=webhook_worker, args=(webhook_queue,))

    storage_process.start()
    webhook_process.start()

    camera_processes = []

    # Start camera streaming threads/processes
    for cam_cfg in config.KAMERALAR:
        logger.info(f"Spawning camera listener for: {cam_cfg['ad']} ({cam_cfg['ip']})")
        p = Process(
            target=run_camera,
            args=(cam_cfg, storage_queue, webhook_queue)
        )
        p.start()
        camera_processes.append(p)

    try:
        # Keep main thread alive
        for p in camera_processes:
            p.join()
    except KeyboardInterrupt:
        logger.info("Stopping Platar Alarm Service...")
        storage_process.terminate()
        webhook_process.terminate()
        for p in camera_processes:
            p.terminate()
        logger.info("Platar Alarm Service stopped.")
