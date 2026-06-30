# Platar — Genel Bilgi ve Servis Mimarisi

Bu döküman, **Platar** plaka tanima ve gözetim sisteminin genel mimarisini, veri akis şemasini ve sistemdeki 5 temel servisin ayrintili görev paylasimini açiklar.

---

## 🏛️ 1. Proje Genel Mimarisi

Platar, kurumsal gözetim standartlarina (Hikvision HikCentral vb.) uygun sekilde tasarlanmis, milisaniyeler düzeyinde plaka tespiti yapan ve bunlari canlı izleme konsoluna yansitan endüstriyel bir yazilim çözümüdür.

Sistem, gevsek bagli (loosely coupled) ve asenkron iletisim kuran iki ana katmandan olusur:
1. **Python Alarm Servisi:** Kamera alert stream baglantilarini yöneten ve ham görsel veriyi isleyen hızlı istemci katmanı.
2. **ASP.NET Core WebUI (C#):** Veri bütünlügü, kuyruk yönetimi, canlı SignalR yayını, JWT/Identity güvenligi ve plaka yönetimi ekranlarindan sorumlu ana backend katmanı.

---

## ⚙️ 2. Servisler ve Görevleri

Sistemde kesintisiz sekilde çalisan ve birbirleriyle iletisim halinde olan 5 servis bulunmaktadir:

### 1. ASP.NET Core WebUI (C#)
* **Görevi:** Sistemdeki tüm REST API endpoint'lerini barındırır. Web paneli (Razor + Javascript) ve JWT tabanlı kimlik doğrulama servislerini (`/api/auth/login`) yönetir.
* **Veri Yönetimi:** Gelişmiş filtreleme, sayfalama ve plaka yönetimi işlemlerinin veritabanına kaydedilmesini sağlar.
* **SignalR:** İşlenen plaka geçişlerini anında tarayıcılara yayınlar.

### 2. Python Alarm Servisi (Python)
* **Görevi:** Sistemdeki aktif kameraların HTTP Alert Stream (multipart/mixed) akışlarını kesintisiz dinler.
* **Çoklu İş parçacığı (Multi-threading):** Her kamera bağlantısı için hafif bir thread (`threading.Thread`) açarak kaynak tüketimini azaltır.
* **İşçi Süreçleri (Multi-processing):** Görüntü dönüştürme (Pillow) ve MinIO yükleme işlemlerini paralel yürüten bağımsız işçi süreçleri (worker pool) kullanır.
* **Entegrasyon:** Görselleri MinIO'ya yükledikten sonra WebUI webhook API'sini tetikler.

### 3. Hangfire Arka Plan İşçisi (C# Background Jobs)
* **Görevi:** Web uygulamasının performansını korumak amacıyla ağır veritabanı veya MinIO işlemlerini (Örn: presigned url oluşturma, plaka kategorisi eşleştirme) arka plana alır.
* **SignalR Entegrasyonu:** İşlem tamamlandığında SignalR Hub üzerinden tarayıcıya "NewPlateDetected" yayını gönderir.

### 4. MinIO Object Storage
* **Görevi:** Kameralardan gelen yüksek çözünürlüklü araç ve plaka fotoğraflarını güvenli şekilde saklar.
* **Güvenlik (Presigned URL):** Arayüzde görüntülenecek resimler için 15 dakika geçerliliği olan imzalı geçici bağlantılar üreterek dosya güvenliğini sağlar.

### 5. MediaMTX Video Server
* **Görevi:** Kameraların RTSP (Real-Time Streaming Protocol) video akışlarını toplar.
* **HLS Dönüşümü:** RTSP akışını web tarayıcıların oynatabileceği HLS (HTTP Live Streaming) formatına dönüştürür.

---

## 🔄 3. Veri Akış Aşamaları

```text
[Kamera] ── (Alert Stream) ──> [Python Servisi] ── (Görsel Yükleme) ──> [MinIO Storage]
                                      │
                               (Webhook Tetikleme)
                                      ▼
[Kullanıcı Paneli] <── (SignalR) ── [WebUI (C#)] ── (Hangfire Job) ──> [PostgreSQL DB]
```

1. Kamera bir plaka okuduğunda **Python Alarm Servisi**'ne alert paketini yollar.
2. Python servisi resimleri **MinIO**'ya yükler ve **WebUI Webhook**'unu tetikler.
3. WebUI, Hangfire kuyruğuna yeni bir iş ekler.
4. Hangfire işçisi plakayı **Vehicles** tablosunda sorgular (yoksa **Normal** olarak kaydeder) ve resmi **SignalR** ile canlı panele yollar.
