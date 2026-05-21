using System.Text;
using System.Xml.Linq;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Application.Models;

namespace PlakaTanima.Alarm.Parsers;

public class HikvisionXmlParser : IAlarmParser
{
    public string Brand => "Hikvision";

    public CameraAlarmAlert ParseMultipart(byte[] multipartBytes, string cameraName)
    {
        var alert = new CameraAlarmAlert { CameraName = cameraName };
        
        byte[] jpegStart = new byte[] { 0xFF, 0xD8 };
        byte[] jpegEnd = new byte[] { 0xFF, 0xD9 };

        string totalContent = Encoding.UTF8.GetString(multipartBytes);
        
        // Hikvision'ın kullandığı standart multipart boundary kelimelerine göre bölüyoruz
        string[] parts = totalContent.Split(new string[] { "--boundary", "--boundary--" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            if (string.IsNullOrWhiteSpace(part)) continue;

            // 1. KISIM: XML VERİSİ
            if (part.Contains("Content-Type: application/xml") || part.Contains("<EventNotificationAlert"))
            {
                int xmlStartIndex = part.IndexOf("<EventNotificationAlert");
                if (xmlStartIndex != -1)
                {
                    string xmlText = part.Substring(xmlStartIndex);
                    ParseXmlFields(xmlText, alert);
                }
            }
            // 2. KISIM: GÖRSELLER (JPEG)
            // Hikvision multipart akışında, binary parçanın başındaki text header'da Content-Description alanları bulunur
            else if (part.Contains("Content-Type: image/jpeg"))
            {
                string description = "unknown";
                
                // XML'deki pictureInfo sırasına ve Content-Description başlıklarına göre ayırıyoruz
                if (part.Contains("Content-Description: licensePlatePicture") || part.Contains("licensePlatePicture.jpg")) 
                    description = "plate";
                else if (part.Contains("Content-Description: vehiclePicture") || part.Contains("vehiclePicture.jpg")) 
                    description = "vehicle";
                else if (part.Contains("Content-Description: detectionPicture") || part.Contains("detectionPicture.jpg")) 
                    description = "scene";

                byte[] partBytes = Encoding.UTF8.GetBytes(part);
                
                // Orijinal ham byte yığınından resmi bozmadan binary olarak kesip çıkarıyoruz
                byte[]? imageBytes = ExtractJpegBytes(multipartBytes, partBytes[..Math.Min(partBytes.Length, 300)], jpegStart, jpegEnd);

                if (imageBytes != null)
                {
                    if (description == "plate") alert.PlateImageBytes = imageBytes;
                    else if (description == "vehicle") alert.VehicleImageBytes = imageBytes;
                    else if (description == "scene") alert.FullSceneImageBytes = imageBytes;
                }
            }
        }

        return alert;
    }

    private void ParseXmlFields(string xmlContent, CameraAlarmAlert alert)
    {
        try
        {
            var xDoc = XDocument.Parse(xmlContent);
            XNamespace ns = xDoc.Root?.GetDefaultNamespace() ?? XNamespace.None;

            // XML örneğine göre nokta atışı eşlemeler:
            
            // Kamera İsmi: <channelName>30 AGUSTOS GIRIS PLATAR</channelName>
            string xmlChannelName = xDoc.Descendants(ns + "channelName").FirstOrDefault()?.Value ?? string.Empty;
            if (!string.IsNullOrEmpty(xmlChannelName))
            {
                alert.CameraName = xmlChannelName;
            }

            // <ANPR> düğümünü hedef alıyoruz
            var anprNode = xDoc.Descendants(ns + "ANPR").FirstOrDefault();
            if (anprNode != null)
            {
                // Plaka No: <licensePlate>38AAP084</licensePlate>
                alert.PlateNumber = anprNode.Element(ns + "licensePlate")?.Value ?? "Unknown";
                
                // Araç Tipi: <vehicleType>bus</vehicleType>
                alert.VehicleType = anprNode.Element(ns + "vehicleType")?.Value ?? "Unknown";
                
                // Yön: <direction>forward</direction>
                alert.MovingDirection = anprNode.Element(ns + "direction")?.Value ?? "Forward";
                
                // Kütüphane Adı: <vehicleListName>otherList</vehicleListName>
                alert.ListLibraryName = anprNode.Element(ns + "vehicleListName")?.Value ?? "otherList";
                
                // Ülke Kodu: <country>46</country> (Sorgunda Turkey demiştin, default Turkey kalabilir veya kodu basabiliriz)
                string countryCode = anprNode.Element(ns + "country")?.Value ?? "46";
                alert.Country = countryCode == "46" ? "Turkey" : countryCode;

                // Plaka Rengi: <plateColor>unknown</plateColor>
                alert.PlateColor = anprNode.Element(ns + "plateColor")?.Value ?? "Unknown";

                // <vehicleInfo> alt düğümünden araç rengini ve markasını alıyoruz
                var vehicleInfo = anprNode.Element(ns + "vehicleInfo");
                if (vehicleInfo != null)
                {
                    // Araç Rengi: <color>green</color>
                    alert.VehicleColor = vehicleInfo.Element(ns + "color")?.Value ?? "Unknown";
                    
                    // Araç Marka Logosu Kodu: <vehicleLogoRecog>1707</vehicleLogoRecog> (Örn: Renault)
                    string logoCode = vehicleInfo.Element(ns + "vehicleLogoRecog")?.Value ?? "Unknown";
                    alert.VehicleBrand = logoCode == "1707" ? "Renault" : logoCode;
                }
            }

            // Zaman: <dateTime>2026-05-21T16:10:06+08:00</dateTime>
            if (DateTime.TryParse(xDoc.Descendants(ns + "dateTime").FirstOrDefault()?.Value, out var parsedTime))
                alert.EventTime = parsedTime;
            else
                alert.EventTime = DateTime.UtcNow;
        }
        catch 
        {
            // XML formatı hatalı gelirse uygulamanın çökmesini engelliyoruz
        }
    }

    private byte[]? ExtractJpegBytes(byte[] source, byte[] searchHeader, byte[] startSign, byte[] endSign)
    {
        int headerPos = FindPattern(source, searchHeader);
        if (headerPos == -1) return null;

        int startPos = FindPattern(source, startSign, headerPos);
        if (startPos == -1) return null;

        int endPos = FindPattern(source, endSign, startPos);
        if (endPos == -1) return null;

        int length = (endPos + 2) - startPos;
        byte[] imageBytes = new byte[length];
        Buffer.BlockCopy(source, startPos, imageBytes, 0, length);
        
        return imageBytes;
    }

    private int FindPattern(byte[] source, byte[] pattern, int startOffset = 0)
    {
        for (int i = startOffset; i <= source.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return i;
        }
        return -1;
    }
}