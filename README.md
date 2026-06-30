# Platar — High-Performance Real-Time ANPR Surveillance Console

Platar; çoklu kamera ağları üzerinden gelen plaka tanıma olaylarını (ANPR/LPR) eşzamanlı (concurrent) olarak dinleyen, işleyen, kategorize eden (VIP, Kara Liste, Personel) ve canlı izleme paneline anlık yansıtan profesyonel bir endüstriyel güvenlik konsolu uygulamasıdır. 

Tasarımı ve kullanıcı deneyimi **Hikvision HikCentral** ve **iVMS** kurumsal izleme panelleri referans alınarak, koyu renk temalı (dark mode) ve yüksek kontrastlı bir endüstriyel arayüz diliyle inşa edilmiştir.

---

## 🛠️ Teknoloji Yığını ve Mimari

Platar, temiz kod prensiplerine ve yüksek ölçeklenebilirliğe uygun olarak **Onion Architecture** ve **Çoklu Süreç/Thread (Multi-Processing/Threading)** modelleriyle geliştirilmiştir.

### 1. ASP.NET Core & Clean Architecture (C# - .NET 8)
* **Domain & Application Katmanları:** İş kuralları, veri modelleri, DTO'lar, arayüz sözleşmeleri (interfaces) ve CQRS MediatR işleyicileri.
* **Persistence (EF Core & PostgreSQL):** PostgreSQL veritabanı şeması, benzersiz indeksler (`Plate` unique index), veri tohumlama (`DbSeeder.cs`) ve otomatik veritabanı göçleri (migration).
* **Infrastructure (MinIO Storage):** Kamera görüntülerinin (araç, plaka, tam sahne) saklanması ve frontend ekranlarına kısa süreli geçici bağlantılarla (**Presigned URL**) güvenli bir şekilde sunulması.
* **SignalR:** Plaka geçiş olaylarının canlı izleme konsoluna **milisaniyeler içinde** gecikmesiz olarak yansıtılması.
* **Hangfire:** Webhook ile gelen olayların kuyruğa alınması, plaka-kategori kontrolleri ve MinIO presigned URL üretimlerinin arka plan işçileri (background jobs) tarafından asenkron yönetilmesi.

### 2. Python Alarm Servisi (Eşzamanlı Kamera Dinleyici)
* **Lightweight Threading:** Kameraların HTTP Multipart boundary alert stream bağlantılarını OS düzeyinde hafif thread'ler (`threading.Thread`) ile dinler. 20+ kamera için minimum bellek (RAM) ve CPU tüketimi sunar.
* **Multi-Process Worker Pool:** Kuyruğa (`multiprocessing.Queue`) düşen plaka tespit olaylarını paralel tüketen bağımsız işçi süreçleri (3 adet `storage_worker`, 2 adet `webhook_worker`).
* **Pillow Image Processing:** Yakalanan JPEG plaka ve araç fotoğraflarını bellek üzerinde PNG formatına dönüştürür.
* **MinIO Client & Webhook:** Dosyaları lokal MinIO nesne deposuna yükler ve ASP.NET Webhook API'sine asenkron bildirim gönderir.

### 3. Docker Altyapısı
* **MediaMTX (RTSP/HLS Stream):** Kameraların RTSP canlı akışlarını HLS formatına dönüştürerek web arayüzünde sürükle-bırak canlı izleme olanağı sağlar.
* **MinIO Object Storage:** API tabanlı lokal görsel depolama motoru (port 9000/9001).

---

## 🌟 Son Güncellemeler ve Yeni Özellikler

Uygulamanın son sürümünde aşağıdaki gelişmiş yetenekler ve güvenlik katmanları sisteme entegre edilmiştir:

### 1. Kimlik Doğrulama & Güvenlik (Identity & JWT)
* **ASP.NET Core Identity:** Kullanıcı ve rol yönetim altyapısı kurulmuş, tüm güvenlik tabloları PostgreSQL veritabanına eklenmiştir.
* **Varsayılan Yönetici Seeding:** Uygulama ilk çalıştığında `admin@gmail.com` (şifre: `admin123`) kullanıcısı otomatik olarak veritabanına tohumlanır.
* **JWT Bearer Authentication:** API koruması ve istemciler için `/api/auth/login` endpoint'i üzerinden 3 saat geçerli JWT token üretimi sağlanmıştır.

### 2. Canlı Plaka Otomatik Kayıt Mekanizması
* **Otomatik Plaka Ekleme:** Kameralardan geçen yeni ve veritabanında henüz kayıtlı olmayan herhangi bir plaka algılandığında, arka plan işçisi (`LprProcessingJob.cs`) tarafından otomatik olarak **Normal** kategoriyle (`Model: Bilinmeyen Araç`, `Sahip: Bilinmeyen Sürücü`) plaka yönetimi tablosuna kaydedilir. Böylece yeni araçlar için manuel kayıt açma zorunluluğu ortadan kalkar.

### 3. Gelişmiş Sorgu & Sayfalama (Pagination)
* **Dinamik UX:** Arama sonuçları ilk etapta 50 kayıt listeler. Altındaki **Daha Fazla Göster** butonu ile sayfa yenilenmeden dinamik olarak sonraki 25 kayıt çekilerek listeye eklenir.
* **Gerçek Zamanlı Filtreleme:** Sol filtre panelindeki tarih aralıkları veya durum seçimleri değiştirildiği anda veritabanına anında sorgu gönderilir.

### 4. Plaka Yönetimi Hızlı İşlemleri (Plate Management UX)
* **Tek Tıkla VIP & Kara Liste:** Tablo satırlarına entegre edilen yıldız (VIP) ve ban (Kara Liste) hızlı butonları sayesinde araçlar tek tıkla listelere eklenip çıkarılabilir, istatistikler ve rozetler anında güncellenir.
* **Hızlı Plaka Girişi:** "Yeni Araç" modalında Marka/Model ve Sürücü alanları varsayılan olarak "Bilinmiyor" değeriyle yüklendiğinden, sadece plaka yazıp kaydetmek yeterlidir.

---

## 📂 Proje Dizin Yapısı

```text
PlakaTanima/Platar/
├── Core/
│   ├── PlakaTanima.Domain/         # Veritabanı Varlıkları, Enumlar
│   └── PlakaTanima.Application/    # DTO'lar, Repository Arayüzleri, Servisler
├── Infrastructure/
│   ├── PlakaTanima.Persistence/    # AppDbContext, Veri Tohumlama (Seed), Migrations
│   ├── PlakaTanima.Infrastructure/ # MinioStorage, Hangfire Jobs, ANPR modülleri
│   └── PlakaTanima.SignalR/        # PlateHub SignalR Canlı Yayın Hub
├── Presentation/
│   └── PlakaTanima.WebUI/          # ASP.NET Core MVC, Razor Views, Controllerlar, Static Assets (wwwroot)
├── Infrastructure/
│   └── PlakaTanima.AlarmService/   # Python Eşzamanlı Kamera Alert Stream Dinleyici
├── docker-compose.yml              # MediaMTX ve MinIO Docker Servisleri
├── mediamtx.yml                    # RTSP Stream Yayın Yapılandırması
├── .env                            # Ortak Çevre Değişkenleri ve Gizli Değerler
└── README.md                       # Geliştirici Kılavuzu
```

---

## ⚙️ Kurulum ve Çalıştırma

Projenizi iki farklı yöntemle ayağa kaldırabilirsiniz: **Windows Servisi Olarak (Windows Server Production Ortamı)** veya **Docker ile (Geliştirme/Linux Ortamı)**.

### 🚀 Yöntem A: Windows Servisleri ile Yayına Alma (Windows Server - Önerilen / Docker'sız)

Windows Server 2019/2022 gibi Docker Desktop desteği olmayan veya Linux konteynerlerinin kararlı çalışmadığı Windows sunucularda, projenin tüm bileşenlerini yerel (native) Windows servisleri olarak arka planda çalıştırabilirsiniz.

Proje kök dizininde bulunan [setup-windows-services.ps1](file:///home/mehmet/Desktop/Platar/setup-windows-services.ps1) betiği bu kurulumu tamamen otomatikleştirir:
1. **NSSM (Non-Sucking Service Manager)** aracını indirir.
2. **MinIO** ve **MediaMTX** yerel Windows (.exe) uygulamalarını indirir ve yapılandırır.
3. WebUI projesini `Release` modda derleyip (`publish`) yayına hazırlar.
4. Python bağımlılıklarını (`requirements.txt`) yükler.
5. Tüm bileşenleri Windows Servisi olarak kaydeder ve arka planda otomatik başlatır.

**Nasıl Çalıştırılır?**
1. Kök dizindeki `.env` dosyasını sunucunuzun PostgreSQL şifresine ve ağ ayarlarına göre doldurun.
2. PowerShell'i **Yönetici Olarak Çalıştır** (Run as Administrator) seçeneği ile açın.
3. Proje dizinine gidip betiği çalıştırın:
   ```powershell
   .\setup-windows-services.ps1
   ```
4. Kurulum bittiğinde, Windows **Servisler** (`services.msc`) panelinden servisleri (`Platar-MinIO`, `Platar-MediaMTX`, `Platar-WebUI`, `Platar-PythonAlarm`) yönetebilirsiniz. Sunucu kapansa bile bu servisler arka planda otomatik olarak ayağa kalkacaktır.

---

### Yöntem B: Docker ile Çalıştırma (Geliştirme / Linux Ortamı)

#### 1. Ön Gereksinimler
* [Docker Desktop](https://www.docker.com/) yüklü ve çalışır durumda olmalıdır.
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) yüklü olmalıdır.
* [Python 3.10+](https://www.python.org/downloads/) ve `pip` kurulu olmalıdır.
* PostgreSQL sunucusu çalışıyor olmalıdır.

#### 2. Docker Servislerini Başlatın
Kök dizinde terminal açarak MediaMTX ve MinIO nesne depolarını ayağa kaldırın:
```bash
docker compose up -d
```

### 3. Çevre Değişkenlerini Düzenleyin
Kök dizindeki `.env` dosyasını kendi PostgreSQL şifrenize ve ağ yapılandırmanıza göre düzenleyin:
```env
# --- ASP.NET Core (C#) ---
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=platar-test-db;Username=postgres;Password=admin123

# --- PostgreSQL (Python) ---
DB_HOST=localhost
DB_PORT=5432
DB_NAME=platar-test-db
DB_USER=postgres
DB_PASS=admin123

# --- MinIO (Ortak) ---
MINIO_ENDPOINT=localhost:9000
MINIO_ACCESS_KEY=minioadmin
MINIO_SECRET_KEY=minioadmin
MINIO_BUCKET=platar-bucket
MINIO_SECURE=False
```

### 4. ASP.NET Core Web Uygulamasını Başlatın
WebUI projesini çalıştırdığınızda veritabanı otomatik olarak oluşturulacak, EF migrations uygulanacak ve test kameraları, lokasyonları ile test araçları (VIP, Kara Liste) otomatik olarak tohumlanacaktır (seed).
```bash
cd Presentation/PlakaTanima.WebUI
dotnet run
```
* **Web Arayüzü:** `http://localhost:5233`
* **Hangfire Panel:** `http://localhost:5233/hangfire`

### 5. Python Alarm Servisini Başlatın
Python servisinin bağımlılıklarını kurun ve çalıştırın:
```bash
cd Infrastructure/PlakaTanima.AlarmService
pip install -r requirements.txt
python main.py
```
*Servis başladığında PostgreSQL veritabanındaki aktif kameraları okuyarak her biri için dinleyici thread'lerini ayağa kaldıracak, test plaka yakalamalarını veritabanına ve MinIO'ya yükleyerek WebWebhook tetikleyecektir.*

---

## 📊 Örnek Test Plakaları (Seed Verileri)
Uygulama ilk başladığında veritabanına otomatik eklenen bazı test plakaları ve arayüzdeki kategorileri:
* `34 ABC 123` - VIP (Mercedes E200 - Sol kenarı sarı kart olarak canlı yayına düşer)
* `06 XYZ 987` - KARA LİSTE (BMW X5 - Sol kenarı kırmızı kart olarak canlı yayına düşer)
* `21 AAA 001` - PERSONEL (Volkswagen Passat - Mavi rozetli olarak canlı yayına düşer)
* `16 NMG 772` - Normal (Toyota Corolla)
