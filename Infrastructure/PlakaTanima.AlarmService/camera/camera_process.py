import time
import requests
from requests.auth import HTTPDigestAuth

from camera.multipart_parser import MultipartParser
from camera.event_correlator import EventCorrelator
from utils.logger import logger
import config

class CameraProcess:

    def __init__(self, camera_config, storage_queue, webhook_queue):
        self.cfg = camera_config
        self.storage_queue = storage_queue
        self.webhook_queue = webhook_queue

        self.parser = MultipartParser()
        self.correlator = EventCorrelator()

        self.url = f"http://{self.cfg['ip']}/ISAPI/Event/notification/alertStream"
        self.running = True

    def stop(self):
        """
        Gracefully signals the worker loop to terminate.
        """
        self.running = False
        logger.info(f"[{self.cfg['ad']}] Stop signal received. Exiting thread...")

    def start(self):
        auth = HTTPDigestAuth(self.cfg['user'], self.cfg['pass'])

        while self.running:
            try:
                logger.info(f"[{self.cfg['ad']}] Connecting to {self.url} ...")

                response = requests.get(
                    self.url,
                    auth=auth,
                    stream=True,
                    timeout=config.STREAM_TIMEOUT
                )

                response.raise_for_status()
                logger.info(f"[{self.cfg['ad']}] Connected")

                for chunk in response.iter_content(chunk_size=8192):
                    if not self.running:
                        break
                    if not chunk:
                        continue

                    packets = self.parser.feed(chunk)

                    for packet in packets:
                        if not self.running:
                            break
                        if packet["type"] == "xml":
                            self.correlator.handle_xml(
                                self.cfg['ad'],
                                packet["data"]
                            )
                        elif packet["type"] == "jpg":
                            event = self.correlator.handle_jpg(packet["data"])

                            if event:
                                # Put to storage queue which does local save + postgres insert
                                self.storage_queue.put(event)
                                # Optionally put to webhook queue if notification is configured
                                if self.webhook_queue:
                                    self.webhook_queue.put(event)
                                logger.info(f"[{self.cfg['ad']}] Plate parsed and queued: {event.plate}")

            except Exception as e:
                if not self.running:
                    break
                logger.error(f"[{self.cfg['ad']}] Connection lost: {e}")
                
                # Check running flag inside reconnect delay
                for _ in range(config.RECONNECT_DELAY):
                    if not self.running:
                        break
                    time.sleep(1)
