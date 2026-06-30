# Platar — Windows Server Kullanım ve Yönetim Kılavuzu

Bu belge, **Platar** sisteminin Windows Server ortamında nasıl yönetileceğini ve güvenli şekilde kapatılacağını açıklar.

---

## ⚙️ 1. Servis Yönetimi

Sistem Windows Server üzerinde yerel (native) Windows Servisleri olarak çalıştırılmaktadır. Bu servislerin yönetimi için PowerShell veya Windows Servis Yöneticisi arayüzü kullanılabilir.

### Servisler
* **`Platar-MinIO`**: Depolama Sunucusu.
* **`Platar-MediaMTX`**: Medya Akış Sunucusu.
* **`Platar-WebUI`**: ASP.NET Core Web Uygulaması.
* **`Platar-PythonAlarm`**: Python Dinleyici Servisi.

### 💻 A. PowerShell Komutları (Yönetici Olarak Çalıştırın)

* **Servislerin Durumunu Kontrol Etme:**
  ```powershell
  Get-Service Platar-*
  ```

* **Servisleri Başlatma:**
  ```powershell
  Start-Service Platar-MinIO, Platar-MediaMTX, Platar-WebUI, Platar-PythonAlarm
  ```

* **Servisleri Durdurma:**
  ```powershell
  Stop-Service Platar-PythonAlarm, Platar-WebUI, Platar-MediaMTX, Platar-MinIO
  ```

### 🖥️ B. Windows Servis Arayüzü
1. `services.msc` aracını açın.
2. `Platar-` ile başlayan 4 servisi bulun.
3. Sağ tıklayarak durdurup başlatabilirsiniz.

---

## ⚠️ 2. Kritik Kural: Sunucu Kapatılmadan Önce Graceful Shutdown

> [!WARNING]
> Windows Server fiziksel olarak kapatılmadan veya yeniden başlatılmadan önce, tüm Platar servislerinin **manuel olarak durdurulması** gerekmektedir.

### Neden Durdurmalıyız?
1. **Hangfire Kuyrukları:** Aktif arka plan işçilerinin veri kaybına yol açmasını ve PostgreSQL veritabanında kilitli (lock) kalmasını engellemek için.
2. **Alt Süreç Artıkları:** Python alarm servisinin işletim sistemi düzeyinde oluşturduğu paralel işçilerin (multi-processing) düzgün kapanmasını sağlamak ve portların kilitli kalmasını önlemek için.

### Kapatma Sırası
1. **`Platar-PythonAlarm`** servisini durdurun.
2. **`Platar-WebUI`** servisini durdurun.
3. **`Platar-MediaMTX`** servisini durdurun.
4. **`Platar-MinIO`** servisini durdurun.
5. Servislerin durumunun `Stopped` olduğunu `Get-Service Platar-*` ile teyit ettikten sonra sunucuyu kapatın.
