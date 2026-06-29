from dataclasses import dataclass, field
from datetime import datetime
from typing import Dict, List
import uuid

@dataclass
class ANPREvent:
    camera_name: str
    plate: str
    xml_data: dict
    expected_types: List[str]

    event_id: str = field(default_factory=lambda: str(uuid.uuid4()))
    timestamp: str = field(default_factory=lambda: datetime.now().strftime("%Y%m%d_%H%M%S_%f"))

    images: Dict[str, bytes] = field(default_factory=dict)

    image_counter: int = 0
