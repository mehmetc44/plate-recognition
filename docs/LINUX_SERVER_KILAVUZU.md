# Platar — Linux Server Kullanım ve Yönetim Kılavuzu

Bu belge, **Platar** sisteminin Linux Server (Ubuntu/Debian, CentOS/RHEL vb.) üzerinde nasıl kurulacağını, `systemd` servislerinin yapılandırmasını ve güvenli kapatma prosedürlerini açıklar.

---

## ⚙️ 1. Altyapı Servisleri (Docker Compose)

Linux ortamında, depolama (**MinIO**) ve medya akışı (**MediaMTX**) servisleri Docker Compose kullanılarak ayağa kaldırılır.

* **Servisleri Başlatma (Arka Planda):**
  ```bash
  docker compose up -d
  ```

* **Servisleri Durdurma:**
  ```bash
  docker compose stop
  ```

* **Servisleri Tamamen Kapatma ve Temizleme:**
  ```bash
  docker compose down
  ```

---

## 💻 2. Uygulama Servisleri (Systemd)

ASP.NET Core WebUI ve Python Alarm Servisi, Linux işletim sisteminde arka planda kesintisiz çalışması ve sunucu açılışında otomatik başlaması için `systemd` servisleri (unit files) olarak tanımlanmalıdır.

### A. WebUI Servis Dosyası (`/etc/systemd/system/platar-webui.service`)
```ini
[Unit]
Description=Platar WebUI ASP.NET Core Application
After=network.target network-online.target

[Service]
WorkingDirectory=/home/mehmet/Desktop/Platar
ExecStart=/usr/bin/dotnet run --project Presentation/PlakaTanima.WebUI
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=platar-webui
User=mehmet
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

### B. Python Alarm Servis Dosyası (`/etc/systemd/system/platar-pythonalarm.service`)
```ini
[Unit]
Description=Platar Python ANPR Alarm Service
After=network.target platar-webui.service

[Service]
WorkingDirectory=/home/mehmet/Desktop/Platar/Infrastructure/PlakaTanima.AlarmService
ExecStart=/usr/bin/python3 main.py
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=platar-pythonalarm
User=mehmet

[Install]
WantedBy=multi-user.target
```

### C. Servisleri Aktifleştirme ve Başlatma
Servis dosyalarını oluşturduktan sonra aşağıdaki komutlarla aktif hale getirin:

```bash
# Systemd konfigürasyonunu yenileyin
sudo systemctl daemon-reload

# Servisleri başlangıçta açılacak şekilde kaydedin
sudo systemctl enable platar-webui platar-pythonalarm

# Servisleri başlatın
sudo systemctl start platar-webui platar-pythonalarm
```

### D. Servis Durumunu ve Loglarını İnceleme
* **Servislerin Durumu:**
  ```bash
  sudo systemctl status platar-webui platar-pythonalarm
  ```

* **WebUI Loglarını Canlı Takip Etme:**
  ```bash
  sudo journalctl -u platar-webui -f -n 100
  ```

* **Python Alarm Servisi Loglarını Canlı Takip Etme:**
  ```bash
  sudo journalctl -u platar-pythonalarm -f -n 100
  ```

---

## ⚠️ 3. Kritik Kural: Sunucu Kapatılmadan Önce Graceful Shutdown

> [!WARNING]
> Linux Server yeniden başlatılmadan veya kapatılmadan önce, tüm servisler **sırasıyla temiz bir şekilde durdurulmalıdır**.

### Neden Durdurmalıyız?
* **Hangfire Kuyrukları:** Aktif arka plan işlerinin yarıda kalarak veritabanında kilitli kalmaması için.
* **Süreç Yönetimi:** Python çoklu süreçlerinin (workers) işletim sistemi tarafından zorla sonlandırılmasını (SIGKILL) engellemek ve zombi süreçler oluşmasının önüne geçmek için.

### Kapatma Sırası
1. **Systemd Servislerini Durdurun:**
   ```bash
   sudo systemctl stop platar-pythonalarm platar-webui
   ```
2. **Docker Compose Servislerini Durdurun:**
   ```bash
   docker compose stop
   ```
3. Servislerin kapandığından emin olduktan sonra Linux sunucuyu kapatabilirsiniz:
   ```bash
   sudo shutdown -h now
   ```
