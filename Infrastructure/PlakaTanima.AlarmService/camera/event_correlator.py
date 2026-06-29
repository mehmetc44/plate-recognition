import xml.etree.ElementTree as ET
from collections import deque
from models import ANPREvent

class EventCorrelator:

    def __init__(self):
        self.pending_events = deque()

    def parse_xml(self, xml_str):
        root = ET.fromstring(xml_str)

        def xml_to_dict(element):
            tag = element.tag.split('}')[-1]
            children = list(element)

            if not children:
                return element.text

            result = {}
            for child in children:
                child_tag = child.tag.split('}')[-1]
                value = xml_to_dict(child)

                if child_tag in result:
                    if not isinstance(result[child_tag], list):
                        result[child_tag] = [result[child_tag]]
                    result[child_tag].append(value)
                else:
                    result[child_tag] = value
            return result

        data = xml_to_dict(root)
        anpr = data.get("ANPR", {})
        plate = anpr.get("licensePlate")

        pic_info = anpr.get("pictureInfoList", {}).get("pictureInfo", [])
        if not isinstance(pic_info, list):
            pic_info = [pic_info]

        expected_types = [
            p.get("type") for p in pic_info if p.get("type")
        ]

        return plate, data, expected_types

    def handle_xml(self, camera_name, xml_str):
        try:
            plate, data, expected_types = self.parse_xml(xml_str)

            if not plate:
                return

            event = ANPREvent(
                camera_name=camera_name,
                plate=plate,
                xml_data=data,
                expected_types=expected_types
            )

            self.pending_events.append(event)
        except Exception as e:
            print(f"Error parsing XML: {e}")

    def handle_jpg(self, jpg_data):
        if not self.pending_events:
            return None

        event = self.pending_events[0]

        mapping = {
            "licensePlatePicture": "plate",
            "vehiclePicture": "vehicle",
            "detectionPicture": "full"
        }

        if event.image_counter >= len(event.expected_types):
            return None

        img_type = event.expected_types[event.image_counter]
        mapped_type = mapping.get(img_type, "unknown")

        event.images[mapped_type] = jpg_data
        event.image_counter += 1

        if event.image_counter >= len(event.expected_types):
            self.pending_events.popleft()
            return event

        return None
