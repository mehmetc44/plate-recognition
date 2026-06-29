import os
import io
from datetime import datetime
from PIL import Image
import psycopg2

from models import ANPREvent
from camera.event_correlator import EventCorrelator
from services.storage_service import save_event_files
from services.db_service import save_event_to_db, get_connection
from utils.logger import logger

# Kullanıcının gönderdiği gerçek Hikvision ISAPI XML verisi
USER_XML = """<EventNotificationAlert version="2.0" xmlns="http://www.hikvision.com/ver20/XMLSchema">
<ipAddress>10.179.0.81</ipAddress>
<ipv6Address>::ffff:10.179.0.81</ipv6Address>
<portNo>80</portNo>
<protocol>HTTP</protocol>
<macAddress>08:cc:81:77:93:8a</macAddress>
<channelID>1</channelID>
<dateTime>2026-05-21T16:10:06+08:00</dateTime>
<activePostCount>1</activePostCount>
<eventType>ANPR</eventType>
<eventState>active</eventState>
<eventDescription>ANPR</eventDescription>
<channelName>30 AGUSTOS GIRIS PLATAR</channelName>
<ANPR>
<country>46</country>
<licensePlate>38AAP084</licensePlate>
<line>1</line>
<direction>forward</direction>
<confidenceLevel>98</confidenceLevel>
<plateType>unknown</plateType>
<plateColor>unknown</plateColor>
<licenseBright>128</licenseBright>
<dangmark>no</dangmark>
<twoWheelVehicle>no</twoWheelVehicle>
<threeWheelVehicle>no</threeWheelVehicle>
<plateCharBelieve>99,99,99,99,99,99,99,99</plateCharBelieve>
<vehicleType>bus</vehicleType>
<detectDir>8</detectDir>
<detectType>0</detectType>
<alarmDataType>0</alarmDataType>
<vehicleInfo>
<index>7395</index>
<colorDepth>2</colorDepth>
<color>green</color>
<length>0</length>
<vehicleLogoRecog>1707</vehicleLogoRecog>
<vehileSubLogoRecog>0</vehileSubLogoRecog>
<vehileModel>0</vehileModel>
</vehicleInfo>
<pictureInfoList>
<pictureInfo>
<fileName>licensePlatePicture.jpg</fileName>
<type>licensePlatePicture</type>
<dataType>0</dataType>
<picRecogMode>0</picRecogMode>
<absTime>20260521161006471</absTime>
<pId>2026052116100697700VdqwBMgtNT4wY</pId>
</pictureInfo>
<pictureInfo>
<fileName>vehiclePicture.jpg</fileName>
<type>vehiclePicture</type>
<dataType>0</dataType>
<picRecogMode>0</picRecogMode>
<absTime>20260521161006471</absTime>
<pId>2026052116100697700RszGATIng1Npg</pId>
</pictureInfo>
<pictureInfo>
<fileName>detectionPicture.jpg</fileName>
<type>detectionPicture</type>
<dataType>0</dataType>
<picRecogMode>0</picRecogMode>
<absTime>20260521161006471</absTime>
<plateRect>
<X>413</X>
<Y>241</Y>
<width>26</width>
<height>10</height>
</plateRect>
<pId>2026052116100697800O730NEzqs9rvs</pId>
</pictureInfo>
</pictureInfoList>
<originalLicensePlate>38AAP084</originalLicensePlate>
<CRIndex>46</CRIndex>
<vehicleListName>otherList</vehicleListName>
<plateSize>0</plateSize>
</ANPR>
<UUID>2026052116100697300edbIawTkCdRKyBzG1WsY1fXRgsEIWclUtyTfpx1Weqn1</UUID>
<picNum>3</picNum>
<monitoringSiteID></monitoringSiteID>
<isDataRetransmission>false</isDataRetransmission>
<DeviceGPSInfo>
<longitudeType>E</longitudeType>
<latitudeType>N</latitudeType>
<Longitude>
<degree>0</degree>
<minute>0</minute>
<sec>0.000000</sec>
</Longitude>
<Latitude>
<degree>0</degree>
<minute>0</minute>
<sec>0.000000</sec>
</Latitude>
</DeviceGPSInfo>
<detectionBackgroundImageResolution>
<height>1568</height>
<width>2560</width>
</detectionBackgroundImageResolution>
</EventNotificationAlert>"""

def generate_test_jpeg(color):
    """Generates a simple 100x100 JPEG image in memory with a solid color."""
    img = Image.new('RGB', (100, 100), color=color)
    img_byte_arr = io.BytesIO()
    img.save(img_byte_arr, format='JPEG')
    return img_byte_arr.getvalue()

def run_test():
    logger.info("=== Platar Python XML Test Simülasyonu Başlatılıyor ===")

    # 1. XML'i event correlator kullanarak parse et
    logger.info("[ADIM 1] Gerçek Hikvision XML verisi parse ediliyor...")
    correlator = EventCorrelator()
    plate, xml_data, expected_types = correlator.parse_xml(USER_XML)
    
    logger.info(f" -> Plaka: {plate}")
    logger.info(f" -> Beklenen Görseller: {expected_types}")

    # ANPREvent nesnesini oluştur
    event = ANPREvent(
        camera_name="30AgustosGiris",
        plate=plate,
        xml_data=xml_data,
        expected_types=expected_types
    )

    # 2. Test resimlerini üret ve event'e ekle
    plate_jpg = generate_test_jpeg('blue')
    vehicle_jpg = generate_test_jpeg('red')
    full_jpg = generate_test_jpeg('green')

    event.images["plate"] = plate_jpg
    event.images["vehicle"] = vehicle_jpg
    event.images["full"] = full_jpg

    # 3. Disk Üzerine Kayıt ve PNG Dönüşümü Testi (dateTime klasör yapısıyla)
    logger.info("\n[ADIM 2] Lokal disk üzerine hiyerarşik kayıt ve PNG dönüşümü başlatılıyor...")
    saved_paths = save_event_files(event)
    
    print("\nKaydedilen Görsel Yolları (Disk):")
    for img_type, path in saved_paths.items():
        print(f" - {img_type}: {path}")
        full_file_path = os.path.join("Plaka", path)
        if os.path.exists(full_file_path):
            print(f"   [OK] Dosya bulundu ve boyutu: {os.path.getsize(full_file_path)} byte")
        else:
            print(f"   [HATA] Dosya diskte bulunamadı: {full_file_path}")

    # 4. PostgreSQL Veritabanı Kayıt Testi
    logger.info("\n[ADIM 3] PostgreSQL veritabanına doğrudan kayıt testi başlatılıyor...")
    save_event_to_db(event, saved_paths)

    # 5. Veritabanından Kaydı Geri Okuyarak Doğrula
    logger.info("\n[ADIM 4] PostgreSQL veritabanından kaydedilen veriler sorgulanıyor...")
    conn = None
    try:
        conn = get_connection()
        with conn.cursor() as cur:
            cur.execute('SELECT "Id", "Plate", "CameraName", "EventTimestamp", "Confidence", "VehicleType", "VehicleColor", "VehicleBrand", "PlateImagePath", "VehicleImagePath", "FullImagePath" FROM "AnprEvents" WHERE "Id" = %s;', (event.event_id,))
            row = cur.fetchone()
            if row:
                print("\n[BAŞARILI] Veritabanı kaydı bulundu!")
                print(f" - ID: {row[0]}")
                print(f" - Plaka: {row[1]}")
                print(f" - Kamera: {row[2]}")
                print(f" - Olay Zamanı: {row[3]}")
                print(f" - Güven Skoru: {row[4]}")
                print(f" - Araç Tipi: {row[5]}")
                print(f" - Araç Rengi: {row[6]}")
                print(f" - Araç Markası (Logo ID): {row[7]}")
                print(f" - Plaka Resmi Yolu: {row[8]}")
                print(f" - Araç Resmi Yolu: {row[9]}")
                print(f" - Full Resim Yolu: {row[10]}")
            else:
                print("\n[HATA] Veritabanında kayıt bulunamadı!")
    except Exception as e:
        logger.error(f"Veritabanından okuma hatası: {e}")
    finally:
        if conn:
            conn.close()

    logger.info("\n=== Simülasyon Testi Tamamlandı ===")

if __name__ == "__main__":
    run_test()
