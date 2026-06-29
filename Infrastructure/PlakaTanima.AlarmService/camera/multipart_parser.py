class MultipartParser:

    def __init__(self):
        self.buffer = b""

    def feed(self, chunk):
        self.buffer += chunk
        packets = []

        while True:
            idx_xml_start = self.buffer.find(b"<EventNotificationAlert")
            idx_xml_end = self.buffer.find(b"</EventNotificationAlert>")

            idx_jpg_start = self.buffer.find(b"\xff\xd8")
            idx_jpg_end = self.buffer.find(b"\xff\xd9")

            has_xml = idx_xml_start != -1 and idx_xml_end != -1
            has_jpg = idx_jpg_start != -1 and idx_jpg_end != -1

            if not has_xml and not has_jpg:
                break

            if has_xml and (not has_jpg or idx_xml_start < idx_jpg_start):
                end_pos = idx_xml_end + len(b"</EventNotificationAlert>")
                xml_data = self.buffer[idx_xml_start:end_pos]
                self.buffer = self.buffer[end_pos:]

                packets.append({
                    "type": "xml",
                    "data": xml_data.decode("utf-8", errors="ignore")
                })

            else:
                end_pos = idx_jpg_end + 2
                jpg_data = self.buffer[idx_jpg_start:end_pos]
                self.buffer = self.buffer[end_pos:]

                packets.append({
                    "type": "jpg",
                    "data": jpg_data
                })

        if len(self.buffer) > 5_000_000:
            self.buffer = self.buffer[-1_000_000:]

        return packets
