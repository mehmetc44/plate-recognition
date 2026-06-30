# ==============================================================================
# Platar - Windows Services Auto-Setup Script
# ==============================================================================
# Bu betik, Windows Server ortamında Docker kullanmadan tüm Platar bileşenlerini
# (MinIO, MediaMTX, ASP.NET WebUI ve Python Alarm Servisi) otomatik olarak indirir,
# yapılandırır ve Windows Servisi olarak kaydeder.
#
# KULLANIM:
# PowerShell'i YÖNETİCİ (Administrator) olarak açın ve bu dosyayı çalıştırın:
# .\setup-windows-services.ps1
# ==============================================================================

# 0. Yönetici Yetkisi Kontrolü
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Error "CRITICAL: Bu betiği çalıştırmak için PowerShell'i 'Yönetici Olarak Çalıştır' (Run as Administrator) seçeneğiyle açmalısınız!"
    Exit
}

# TLS ayarlarını güncelle (İndirmeler sırasında hata oluşmaması için)
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# Betik dizinini bul
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
if ([string]::IsNullOrEmpty($ScriptDir)) { $ScriptDir = Get-Location }

Write-Host "=== Platar Windows Servis Kurulumu Başlatılıyor ===" -ForegroundColor Green
Write-Host "Çalışma Dizini: $ScriptDir" -ForegroundColor Gray

# Dizin Yolları
$ThirdPartyDir = Join-Path $ScriptDir "ThirdParty"
$MinioDataDir = Join-Path $ScriptDir "minio_data"

if (-not (Test-Path $ThirdPartyDir)) { New-Item -ItemType Directory -Path $ThirdPartyDir | Out-Null }
if (-not (Test-Path $MinioDataDir)) { New-Item -ItemType Directory -Path $MinioDataDir | Out-Null }

$NssmPath = Join-Path $ThirdPartyDir "nssm.exe"
$MinioExePath = Join-Path $ThirdPartyDir "minio.exe"
$MediaMtxDir = Join-Path $ThirdPartyDir "MediaMTX"
$MediaMtxExePath = Join-Path $MediaMtxDir "mediamtx.exe"

# ------------------------------------------------------------------------------
# 1. Bağımlılıkların İndirilmesi ve Kurulması
# ------------------------------------------------------------------------------

# A. NSSM (Non-Sucking Service Manager)
if (-not (Test-Path $NssmPath)) {
    Write-Host "[1/4] NSSM indiriliyor..." -ForegroundColor Cyan
    $NssmZipPath = Join-Path $ThirdPartyDir "nssm.zip"
    Invoke-WebRequest -Uri "https://nssm.cc/release/nssm-2.24.zip" -OutFile $NssmZipPath
    Expand-Archive -Path $NssmZipPath -DestinationPath $ThirdPartyDir -Force
    
    # 64-bit sürümü taşı ve temizle
    $ExtractedNssm = Join-Path $ThirdPartyDir "nssm-2.24\win64\nssm.exe"
    if (Test-Path $ExtractedNssm) {
        Copy-Item -Path $ExtractedNssm -Destination $NssmPath -Force
    }
    Remove-Item -Path (Join-Path $ThirdPartyDir "nssm-2.24") -Recurse -Force
    Remove-Item -Path $NssmZipPath -Force
    Write-Host ">> NSSM başarıyla kuruldu: $NssmPath" -ForegroundColor Green
} else {
    Write-Host ">> NSSM zaten mevcut." -ForegroundColor Gray
}

# B. MinIO
if (-not (Test-Path $MinioExePath)) {
    Write-Host "[2/4] MinIO (.exe) indiriliyor..." -ForegroundColor Cyan
    Invoke-WebRequest -Uri "https://dl.min.io/server/minio/release/windows-amd64/minio.exe" -OutFile $MinioExePath
    Write-Host ">> MinIO başarıyla indirildi: $MinioExePath" -ForegroundColor Green
} else {
    Write-Host ">> MinIO zaten mevcut." -ForegroundColor Gray
}

# C. MediaMTX
if (-not (Test-Path $MediaMtxExePath)) {
    Write-Host "[3/4] MediaMTX indiriliyor..." -ForegroundColor Cyan
    $MediaMtxZipPath = Join-Path $ThirdPartyDir "mediamtx.zip"
    # v1.9.0 stabil sürüm
    Invoke-WebRequest -Uri "https://github.com/bluenviron/mediamtx/releases/download/v1.9.0/mediamtx_v1.9.0_windows_amd64.zip" -OutFile $MediaMtxZipPath
    Expand-Archive -Path $MediaMtxZipPath -DestinationPath $MediaMtxDir -Force
    
    # Proje içindeki mediamtx.yml dosyasını kopyala
    $ProjectMtxYml = Join-Path $ScriptDir "mediamtx.yml"
    if (Test-Path $ProjectMtxYml) {
        Copy-Item -Path $ProjectMtxYml -Destination (Join-Path $MediaMtxDir "mediamtx.yml") -Force
        Write-Host ">> Proje mediamtx.yml dosyası kopyalandı." -ForegroundColor Gray
    }
    Remove-Item -Path $MediaMtxZipPath -Force
    Write-Host ">> MediaMTX başarıyla kuruldu: $MediaMtxExePath" -ForegroundColor Green
} else {
    Write-Host ">> MediaMTX zaten mevcut." -ForegroundColor Gray
}

# ------------------------------------------------------------------------------
# 2. .env Dosyası ve Derleme (Publish) Aşamaları
# ------------------------------------------------------------------------------

# .env Kontrolü
$EnvFile = Join-Path $ScriptDir ".env"
if (-not (Test-Path $EnvFile)) {
    Write-Warning "UYARI: Kök dizinde .env dosyası bulunamadı!"
    Write-Host "Lütfen önce .env dosyasını doldurun ve bu betiği tekrar çalıştırın."
    Exit
}

# WebUI Derleme (Publish)
Write-Host "[4/4] WebUI projesi Release modunda derleniyor (Publish)..." -ForegroundColor Cyan
$WebUiProj = Join-Path $ScriptDir "Presentation\PlakaTanima.WebUI\PlakaTanima.WebUI.csproj"
$PublishDir = Join-Path $ScriptDir "Presentation\PlakaTanima.WebUI\publish"
dotnet publish $WebUiProj -c Release -o $PublishDir

if (-not (Test-Path (Join-Path $PublishDir "PlakaTanima.WebUI.exe"))) {
    Write-Error "CRITICAL: WebUI derlemesi başarısız oldu! Lütfen .NET Core SDK yüklü olduğundan emin olun."
    Exit
}
Write-Host ">> WebUI başarıyla derlendi: $PublishDir" -ForegroundColor Green

# ------------------------------------------------------------------------------
# 3. Windows Servislerinin Kayıt Edilmesi ve Başlatılması
# ------------------------------------------------------------------------------
Write-Host "=== Windows Servisleri Kayıt Ediliyor ===" -ForegroundColor Green

# Eski servisler varsa temizle
Write-Host "Eski servis kalıntıları temizleniyor..." -ForegroundColor Gray
Get-Service -Name "Platar-*" -ErrorAction SilentlyContinue | ForEach-Object {
    Write-Host "Durduruluyor ve Siliniyor: $($_.Name)" -ForegroundColor Yellow
    Stop-Service $_.Name -ErrorAction SilentlyContinue
    & $NssmPath remove $_.Name confirm
}

# A. Platar-MinIO Servisi
Write-Host "Kayıt ediliyor: Platar-MinIO" -ForegroundColor Cyan
& $NssmPath install Platar-MinIO $MinioExePath "server `"$MinioDataDir`" --address :9000 --console-address :9001"
& $NssmPath set Platar-MinIO AppEnvironmentVars "MINIO_ROOT_USER=minioadmin" "MINIO_ROOT_PASSWORD=minioadmin"
& $NssmPath set Platar-MinIO Start SERVICE_AUTO_START
& $NssmPath set Platar-MinIO DisplayName "Platar MinIO Storage Service"
Start-Service Platar-MinIO

# B. Platar-MediaMTX Servisi
Write-Host "Kayıt ediliyor: Platar-MediaMTX" -ForegroundColor Cyan
& $NssmPath install Platar-MediaMTX $MediaMtxExePath
& $NssmPath set Platar-MediaMTX AppDirectory $MediaMtxDir
& $NssmPath set Platar-MediaMTX Start SERVICE_AUTO_START
& $NssmPath set Platar-MediaMTX DisplayName "Platar MediaMTX Streamer"
Start-Service Platar-MediaMTX

# C. Platar-WebUI (C# ASP.NET Core) Servisi
Write-Host "Kayıt ediliyor: Platar-WebUI" -ForegroundColor Cyan
$WebUiExe = Join-Path $PublishDir "PlakaTanima.WebUI.exe"
& $NssmPath install Platar-WebUI $WebUiExe
& $NssmPath set Platar-WebUI AppDirectory $PublishDir
& $NssmPath set Platar-WebUI Start SERVICE_AUTO_START
& $NssmPath set Platar-WebUI DisplayName "Platar ASP.NET Web Console"
Start-Service Platar-WebUI

# D. Platar-PythonAlarm Servisi
Write-Host "Kayıt ediliyor: Platar-PythonAlarm" -ForegroundColor Cyan
$PythonPath = (Get-Command python.exe -ErrorAction SilentlyContinue).Source
if (-not $PythonPath) {
    # Varsayılan yaygın Python konumlarını kontrol et
    $CommonPaths = @(
        "$env:USERPROFILE\AppData\Local\Programs\Python\Python310\python.exe",
        "$env:USERPROFILE\AppData\Local\Programs\Python\Python311\python.exe",
        "$env:USERPROFILE\AppData\Local\Programs\Python\Python312\python.exe",
        "C:\Python310\python.exe",
        "C:\Python311\python.exe",
        "C:\Python312\python.exe"
    )
    foreach ($p in $CommonPaths) {
        if (Test-Path $p) { $PythonPath = $p; break }
    }
}

if (-not $PythonPath) {
    Write-Warning "Python.exe yolu sistemde otomatik bulunamadı!"
    $PythonPath = Read-Host "Lütfen python.exe'nin tam yolunu girin (Örn: C:\Python310\python.exe)"
}

if (Test-Path $PythonPath) {
    $PythonScript = Join-Path $ScriptDir "Infrastructure\PlakaTanima.AlarmService\main.py"
    $PythonDir = Join-Path $ScriptDir "Infrastructure\PlakaTanima.AlarmService"
    
    # Python bağımlılıklarını kur
    Write-Host "Python bağımlılıkları (requirements.txt) yükleniyor..." -ForegroundColor Cyan
    & $PythonPath -m pip install -r (Join-Path $PythonDir "requirements.txt")
    
    & $NssmPath install Platar-PythonAlarm $PythonPath "`"$PythonScript`""
    & $NssmPath set Platar-PythonAlarm AppDirectory $PythonDir
    & $NssmPath set Platar-PythonAlarm Start SERVICE_AUTO_START
    & $NssmPath set Platar-PythonAlarm DisplayName "Platar Python ANPR Alarm Service"
    Start-Service Platar-PythonAlarm
    Write-Host ">> Platar-PythonAlarm başarıyla başlatıldı." -ForegroundColor Green
} else {
    Write-Error "HATA: Python bulunamadığı için Platar-PythonAlarm servisi kurulamadı."
}

# ------------------------------------------------------------------------------
# 4. Sonuç ve Özet
# ------------------------------------------------------------------------------
Write-Host ""
Write-Host "=================================================================" -ForegroundColor Green
Write-Host "  TÜM PLATAR SERVİSLERİ BAŞARIYLA WINDOWS SERVİSİ OLARAK KURULDU!" -ForegroundColor Green -BackgroundColor Black
Write-Host "=================================================================" -ForegroundColor Green
Write-Host "Servis Listesi ve Durumları:" -ForegroundColor Gray
Get-Service -Name "Platar-*" | Format-Table -Property Name, DisplayName, Status -AutoSize

Write-Host "Detaylar:" -ForegroundColor Gray
Write-Host "- Servisleri açıp kapatmak için Windows 'services.msc' panelini kullanabilirsiniz." -ForegroundColor Gray
Write-Host "- Hata günlüklerini (Log) ve çalışma durumunu incelemek için Windows Olay Görüntüleyicisi'ni (Event Viewer) açabilirsiniz." -ForegroundColor Gray
Write-Host "=================================================================" -ForegroundColor Green
