// ==========================================
// AYARLAR SAYFASI - FULL JS (DÜZELTİLMİŞ)
// ==========================================
(function() {
    if (!document.getElementById('locationsTableBody')) return;
    // ---------- SIDEBAR SEKME GEÇİŞİ ----------
    const menuItems = document.querySelectorAll('.menu-item');
    const panels = document.querySelectorAll('.settings-panel');
    function switchTab(tabId) {
        panels.forEach(p => p.classList.remove('active'));
        const activePanel = document.getElementById(`panel-${tabId}`);
        if (activePanel) activePanel.classList.add('active');
        menuItems.forEach(m => m.classList.remove('active'));
        const activeMenuItem = document.querySelector(`.menu-item[data-tab="${tabId}"]`);
        if (activeMenuItem) activeMenuItem.classList.add('active');
        
        // Kamera ayarları sekmesinde "Kaydet" butonunu gizle/göster
        const saveButtons = document.querySelector('.settings-actions');
        if (saveButtons) {
            if (tabId === 'camera') {
                saveButtons.style.display = 'none';
            } else {
                saveButtons.style.display = 'flex';
            }
        }
    }
    menuItems.forEach(item => {
        item.addEventListener('click', () => switchTab(item.getAttribute('data-tab')));
    });

    // ---------- STATİK AYARLAR (Sistem, Görünüm, Kayıt) TOPLU KAYDETME ----------
    const saveBtn = document.getElementById('saveSettingsBtn');
    const resetBtn = document.getElementById('resetSettingsBtn');

    function collectStaticSettings() {
        return {
            language: document.getElementById('sysLang')?.value,
            timezone: document.getElementById('sysTimezone')?.value,
            retention: document.getElementById('sysRetention')?.value,
            blacklistAlert: document.getElementById('notifyBlacklist')?.checked,
            vipAlert: document.getElementById('notifyVip')?.checked,
            soundAlert: document.getElementById('notifySound')?.checked,
            theme: document.querySelector('.theme-card.active')?.dataset.theme || 'light',
            darkMode: document.getElementById('darkModeToggle')?.checked,
            accentColor: document.querySelector('.color-dot.active')?.dataset.accent || 'blue',
            tableDensity: document.querySelector('.density-btn.active')?.dataset.density || 'normal',
            sidebarPosition: document.getElementById('sidebarPosition')?.value,
            animationsEnabled: document.getElementById('animationsEnabled')?.checked,
            continuousRecording: document.getElementById('recContinuous')?.checked,
            eventRecording: document.getElementById('recEvent')?.checked,
            recordingRetention: document.getElementById('recRetention')?.value,
            videoQuality: document.getElementById('recQuality')?.value
        };
    }

    // ==========================================================
    // PROFİL KAYDETME
    // ==========================================================
    function saveProfile() {
      const name = document.getElementById('profileName')?.value.trim();
      const email = document.getElementById('profileEmail')?.value.trim();
      const currentPw = document.getElementById('profileCurrentPw')?.value;
      const newPw = document.getElementById('profileNewPw')?.value;
      const confirmPw = document.getElementById('profileConfirmPw')?.value;
      const emailNotify = document.getElementById('profileEmailNotify')?.checked;
      const browserNotify = document.getElementById('profileBrowserNotify')?.checked;

      // Profil bilgilerini kaydet
      if (name) {
        try { localStorage.setItem('profile_name', name); } catch(e) {}
        // Navbar'daki ismi de güncelle
        document.querySelectorAll('.user-name').forEach(el => el.textContent = name);
        document.querySelectorAll('.dropdown-user-name').forEach(el => el.textContent = name);
      }
      if (email) {
        try { localStorage.setItem('profile_email', email); } catch(e) {}
        document.querySelectorAll('.dropdown-user-email').forEach(el => el.textContent = email);
      }

      // Şifre değiştirme
      if (currentPw || newPw || confirmPw) {
        if (!currentPw) { showToast('Mevcut şifrenizi girin.', 'error'); return; }
        if (!newPw || newPw.length < 3) { showToast('Yeni şifre en az 3 karakter olmalıdır.', 'error'); return; }
        if (newPw !== confirmPw) { showToast('Yeni şifreler eşleşmiyor.', 'error'); return; }
        try { localStorage.setItem('profile_password', newPw); } catch(e) {}
        // Şifre alanlarını temizle
        document.getElementById('profileCurrentPw').value = '';
        document.getElementById('profileNewPw').value = '';
        document.getElementById('profileConfirmPw').value = '';
        showToast('Şifre başarıyla değiştirildi!', 'success');
      }

      // Bildirim tercihleri
      try {
        localStorage.setItem('profile_email_notify', emailNotify ? '1' : '0');
        localStorage.setItem('profile_browser_notify', browserNotify ? '1' : '0');
      } catch(e) {}

      showToast('Profil bilgileri kaydedildi!', 'success');
    }

    // ==========================================================
    // PROFİL BİLGİLERİNİ YÜKLE
    // ==========================================================
    function loadProfile() {
      try {
        const name = localStorage.getItem('profile_name');
        const email = localStorage.getItem('profile_email');
        if (name) document.getElementById('profileName').value = name;
        if (email) document.getElementById('profileEmail').value = email;
      } catch(e) {}
    }

    function showToast(msg, type = 'info') {
        // Toast bildirimi
        const existing = document.querySelector('.settings-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.className = 'settings-toast';
        toast.textContent = msg;
        toast.style.cssText = `
          position: fixed; bottom: 24px; right: 24px; z-index: 9999;
          padding: 14px 24px; border-radius: 10px; font-size: 0.9rem;
          font-weight: 500; font-family: 'Inter', sans-serif;
          box-shadow: 0 8px 24px rgba(0,0,0,0.2);
          animation: toastIn 0.3s ease;
          background: ${type === 'error' ? 'var(--danger)' : 'var(--success)'};
          color: white;
        `;
        document.body.appendChild(toast);
        setTimeout(() => {
          toast.style.opacity = '0';
          toast.style.transform = 'translateY(10px)';
          toast.style.transition = 'all 0.3s ease';
          setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // ==========================================================
    // TEMA SEÇİMİ — PlatarTheme API'sine bağlı
    // ==========================================================

    /**
     * Settings UI'unu mevcut tema durumuna göre senkronize eder.
     * Sayfa yüklendiğinde ve reset sonrası çağrılır.
     */
    function syncThemeUI() {
      if (!window.PlatarTheme) return;

      const { base, dark } = PlatarTheme.getTheme();
      const accent = PlatarTheme.getAccentColor();
      const density = PlatarTheme.getTableDensity();

      // Tema kartları
      document.querySelectorAll('.theme-card').forEach(c => {
        c.classList.toggle('active', c.dataset.theme === base);
      });

      // Koyu varyant toggle
      const darkToggle = document.getElementById('darkModeToggle');
      if (darkToggle) darkToggle.checked = dark;

      // Vurgu rengi
      document.querySelectorAll('.color-dot').forEach(d => {
        d.classList.toggle('active', d.dataset.accent === accent);
      });

      // Tablo yoğunluğu
      document.querySelectorAll('.density-btn').forEach(b => {
        b.classList.toggle('active', b.dataset.density === density);
      });
    }

    // --- Tema kartı tıklama ---
    document.querySelectorAll('.theme-card').forEach(card => {
      card.addEventListener('click', function () {
        document.querySelectorAll('.theme-card').forEach(c => c.classList.remove('active'));
        this.classList.add('active');

        const base = this.dataset.theme;
        const dark = document.getElementById('darkModeToggle')?.checked || false;

        if (window.PlatarTheme) {
          PlatarTheme.setTheme(base, dark);
        }
      });
    });

    // --- Koyu varyant toggle ---
    const darkToggle = document.getElementById('darkModeToggle');
    if (darkToggle) {
      darkToggle.addEventListener('change', function () {
        const activeCard = document.querySelector('.theme-card.active');
        const base = activeCard?.dataset.theme || 'light';
        const dark = this.checked;

        if (window.PlatarTheme) {
          PlatarTheme.setTheme(base, dark);
        }
      });
    }

    // --- Vurgu rengi seçimi ---
    document.querySelectorAll('.color-dot').forEach(dot => {
      dot.addEventListener('click', function () {
        document.querySelectorAll('.color-dot').forEach(d => d.classList.remove('active'));
        this.classList.add('active');

        const color = this.dataset.accent;
        if (window.PlatarTheme) {
          PlatarTheme.setAccentColor(color);
        }
      });
    });

    // --- Tablo yoğunluğu seçimi ---
    document.querySelectorAll('.density-btn').forEach(btn => {
      btn.addEventListener('click', function () {
        document.querySelectorAll('.density-btn').forEach(b => b.classList.remove('active'));
        this.classList.add('active');

        const density = this.dataset.density;
        if (window.PlatarTheme) {
          PlatarTheme.setDensity(density);
        }
      });
    });

    // --- Sayfa yüklendiğinde UI'ı senkronize et ---
    syncThemeUI();
    loadProfile();

    if (saveBtn) {
        saveBtn.addEventListener('click', () => {
            const settings = collectStaticSettings();
            console.log('Kaydedilen ayarlar (mock):', settings);

            // Tema ayarlarını localStorage'a kaydet (PlatarTheme API'si üzerinden)
            if (window.PlatarTheme) {
              PlatarTheme.setTheme(settings.theme, settings.darkMode);
              PlatarTheme.setAccentColor(settings.accentColor);
              PlatarTheme.setDensity(settings.tableDensity);
            }

            // Profil bilgilerini de kaydet (eğer profil sekmesi aktifse)
            saveProfile();

            showToast('Tüm ayarlar kaydedildi!', 'success');
        });
    }
    if (resetBtn) {
        resetBtn.addEventListener('click', () => {
            // Varsayılan değerleri ata
            if (document.getElementById('sysLang')) document.getElementById('sysLang').value = 'tr';
            if (document.getElementById('sysTimezone')) document.getElementById('sysTimezone').value = 'Europe/Istanbul';
            if (document.getElementById('sysRetention')) document.getElementById('sysRetention').value = '90';
            if (document.getElementById('notifyBlacklist')) document.getElementById('notifyBlacklist').checked = true;
            if (document.getElementById('notifyVip')) document.getElementById('notifyVip').checked = false;
            if (document.getElementById('notifySound')) document.getElementById('notifySound').checked = true;

            // Tema varsayılanları — PlatarTheme API üzerinden
            if (window.PlatarTheme) {
              PlatarTheme.setTheme('light', false);
              PlatarTheme.setAccentColor('blue');
              PlatarTheme.setDensity('normal');
            }

            // UI senkronizasyonu
            syncThemeUI();

            if (document.getElementById('sidebarPosition')) document.getElementById('sidebarPosition').value = 'left';
            if (document.getElementById('animationsEnabled')) document.getElementById('animationsEnabled').checked = true;
            if (document.getElementById('recContinuous')) document.getElementById('recContinuous').checked = false;
            if (document.getElementById('recEvent')) document.getElementById('recEvent').checked = true;
            if (document.getElementById('recRetention')) document.getElementById('recRetention').value = '30';
            if (document.getElementById('recQuality')) document.getElementById('recQuality').value = 'Orta (1080p)';

            showToast('Varsayılan ayarlar yüklendi (kaydetmek için butona tıklayın)', 'info');
        });
    }

    // ==========================================================
    // KAMERA & LOKASYON CRUD (ÇİFT TABLO & GERÇEK BACKEND API)
    // ==========================================================
    let locations = [];
    let currentLocationId = null;

    // DOM elemanları
    const locationsTableBody = document.getElementById('locationsTableBody');
    const detailCameraTableBody = document.getElementById('detailCameraTableBody');
    const selectedLocationTitle = document.getElementById('selectedLocationTitle');
    const globalAddLocationBtn = document.getElementById('globalAddLocationBtn');
    const addCameraFromDetailBtn = document.getElementById('addCameraFromDetailBtn');

    // Modal elemanları
    const locationModal = document.getElementById('locationModal');
    const editLocationModal = document.getElementById('editLocationModal');
    const cameraModal = document.getElementById('cameraModal');
    
    let editingLocationId = null;
    let editingCameraLocationId = null;
    let editingCameraId = null;

    if (!locationsTableBody) {
        console.warn('Kamera ayarları DOM elemanları bulunamadı, dinamik kısım atlanıyor.');
    } else {
        // --- Veriyi sunucudan çek ---
        async function loadCameraSettingsData() {
            try {
                const res = await fetch('/Camera/GetTree');
                const rawLocations = await res.json();
                
                // Sunucudan gelen verileri normalize et
                locations = rawLocations.map(loc => ({
                    id: loc.id,
                    name: loc.name,
                    description: loc.description || '',
                    cameras: (loc.cameras || []).map(cam => ({
                        id: cam.id,
                        name: cam.name,
                        ipAddress: cam.ipAddress || '',
                        port: cam.port || 80,
                        username: cam.username || '',
                        password: cam.password || '',
                        streamChannel: cam.streamChannel || 101,
                        status: cam.status || 'offline'
                    }))
                }));

                renderLocationsTable();

                // Önceden seçili lokasyon varsa veya ilk yüklemede otomatik seç
                if (currentLocationId) {
                    const found = locations.find(l => l.id === currentLocationId);
                    if (found) {
                        selectLocation(currentLocationId);
                        return;
                    }
                }

                if (locations.length > 0) {
                    selectLocation(locations[0].id);
                } else {
                    selectLocation(null);
                }
            } catch (err) {
                console.error('Veri yüklenirken hata oluştu:', err);
                showToast('Sunucudan kamera verileri alınamadı!', 'error');
            }
        }

        // --- Lokasyon Tablosu Render ---
        function renderLocationsTable() {
            locationsTableBody.innerHTML = '';
            
            if (locations.length === 0) {
                locationsTableBody.innerHTML = `<tr><td colspan="4" style="text-align: center; color: var(--text-secondary); padding: 24px;">Kayıtlı lokasyon bulunmuyor.</td></tr>`;
                return;
            }

            locations.forEach(loc => {
                const row = document.createElement('tr');
                row.dataset.id = loc.id;
                if (currentLocationId === loc.id) {
                    row.classList.add('selected-row');
                }

                row.innerHTML = `
                    <td><strong>${loc.name}</strong></td>
                    <td>${loc.description || '<span style="opacity: 0.5;">Açıklama yok</span>'}</td>
                    <td style="text-align: center;"><span class="badge online" style="font-family: monospace;">${loc.cameras.length}</span></td>
                    <td style="text-align: center;" class="action-cell">
                        <button class="action-btn edit" title="Düzenle" data-id="${loc.id}"><i class="fas fa-edit"></i></button>
                        <button class="action-btn delete" title="Sil" data-id="${loc.id}"><i class="fas fa-trash"></i></button>
                    </td>
                `;

                // Satır tıklama
                row.addEventListener('click', (e) => {
                    // Buton tıklamalarında satır seçimini tetikleme
                    if (e.target.closest('.action-btn') || e.target.closest('.action-cell')) return;
                    selectLocation(loc.id);
                });

                // Düzenle butonu
                row.querySelector('.action-btn.edit').addEventListener('click', (e) => {
                    e.stopPropagation();
                    openEditLocationModal(loc);
                });

                // Sil butonu
                row.querySelector('.action-btn.delete').addEventListener('click', async (e) => {
                    e.stopPropagation();
                    if (loc.cameras.length > 0) {
                        alert('İçinde kamera bulunan bir lokasyonu silemezsiniz. Lütfen önce kameraları silin.');
                        return;
                    }
                    if (confirm(`"${loc.name}" lokasyonunu silmek istediğinize emin misiniz?`)) {
                        try {
                            await CameraService.deleteLocation(loc.id);
                            showToast('Lokasyon başarıyla silindi.', 'success');
                            if (currentLocationId === loc.id) {
                                currentLocationId = null;
                            }
                            await loadCameraSettingsData();
                        } catch (err) {
                            showToast(err.message || 'Lokasyon silinemedi.', 'error');
                        }
                    }
                });

                locationsTableBody.appendChild(row);
            });
        }

        // --- Lokasyon Seçimi ---
        function selectLocation(locationId) {
            currentLocationId = locationId;

            // Tablodaki seçili sınıfını güncelle
            document.querySelectorAll('#locationsTableBody tr').forEach(tr => {
                if (tr.dataset.id === locationId) {
                    tr.classList.add('selected-row');
                } else {
                    tr.classList.remove('selected-row');
                }
            });

            if (!locationId) {
                if (selectedLocationTitle) selectedLocationTitle.textContent = 'Kameralar';
                if (addCameraFromDetailBtn) addCameraFromDetailBtn.disabled = true;
                if (detailCameraTableBody) {
                    detailCameraTableBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: var(--text-secondary); padding: 24px;">Lütfen yukarıdan bir lokasyon seçin.</td></tr>`;
                }
                return;
            }

            const loc = locations.find(l => l.id === locationId);
            if (!loc) return;

            if (selectedLocationTitle) selectedLocationTitle.textContent = `Kameralar [${loc.name}]`;
            if (addCameraFromDetailBtn) addCameraFromDetailBtn.disabled = false;

            renderCamerasTable(loc);
        }

        // --- Kameralar Tablosu Render ---
        function renderCamerasTable(location) {
            detailCameraTableBody.innerHTML = '';

            if (location.cameras.length === 0) {
                detailCameraTableBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: var(--text-secondary); padding: 24px;">Bu lokasyona ait kayıtlı kamera bulunmuyor.</td></tr>`;
                return;
            }

            location.cameras.forEach(cam => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td style="font-family: monospace; font-size: 0.8rem;">${cam.id}</td>
                    <td><strong>${cam.name}</strong></td>
                    <td style="font-family: monospace;">${cam.ipAddress}</td>
                    <td style="font-family: monospace;">${cam.port}</td>
                    <td style="text-align: center;"><span class="badge ${cam.status}">${cam.status === 'online' ? 'Çevrimiçi' : 'Çevrimdışı'}</span></td>
                    <td style="text-align: center;">
                        <button class="action-btn edit" title="Düzenle" data-id="${cam.id}"><i class="fas fa-edit"></i></button>
                        <button class="action-btn delete" title="Sil" data-id="${cam.id}"><i class="fas fa-trash"></i></button>
                    </td>
                `;

                // Düzenle butonu
                row.querySelector('.action-btn.edit').addEventListener('click', () => {
                    openCameraModal(cam, location.id);
                });

                // Sil butonu
                row.querySelector('.action-btn.delete').addEventListener('click', async () => {
                    if (confirm(`"${cam.name}" kamerasını silmek istediğinize emin misiniz?`)) {
                        try {
                            await CameraService.deleteCamera(cam.id);
                            showToast('Kamera başarıyla silindi.', 'success');
                            await loadCameraSettingsData();
                        } catch (err) {
                            showToast(err.message || 'Kamera silinemedi.', 'error');
                        }
                    }
                });

                detailCameraTableBody.appendChild(row);
            });
        }

        // --- Lokasyon Ekle Modal Açar ---
        function openLocationModal() {
            if (!locationModal) return;
            document.getElementById('locationNameInput').value = '';
            document.getElementById('locationDescInput').value = '';
            locationModal.style.display = 'flex';
        }

        function closeLocationModal() {
            if (locationModal) locationModal.style.display = 'none';
        }

        // --- Lokasyon Düzenle Modal Açar ---
        function openEditLocationModal(loc) {
            if (!editLocationModal) return;
            editingLocationId = loc.id;
            document.getElementById('editLocationId').value = loc.id;
            document.getElementById('editLocationName').value = loc.name;
            document.getElementById('editLocationDesc').value = loc.description || '';
            editLocationModal.style.display = 'flex';
        }

        function closeEditLocationModal() {
            if (editLocationModal) editLocationModal.style.display = 'none';
        }

        // --- Kamera Ekle/Düzenle Modal Açar ---
        function openCameraModal(cam = null, locationId) {
            if (!cameraModal) return;
            editingCameraLocationId = locationId;
            editingCameraId = cam ? cam.id : null;

            document.getElementById('cameraId').value = editingCameraId || '';
            document.getElementById('cameraLocationId').value = editingCameraLocationId || '';
            document.getElementById('cameraNameInput').value = cam ? cam.name : '';
            document.getElementById('cameraIpInput').value = cam ? cam.ipAddress : '';
            document.getElementById('cameraPortInput').value = cam ? cam.port : 80;
            document.getElementById('cameraUsernameInput').value = cam ? cam.username : '';
            document.getElementById('cameraPasswordInput').value = cam ? cam.password : '';
            cameraModal.style.display = 'flex';
        }

        function closeCameraModal() {
            if (cameraModal) cameraModal.style.display = 'none';
        }

        // --- Event Listener'lar ---
        if (globalAddLocationBtn) globalAddLocationBtn.addEventListener('click', openLocationModal);
        if (addCameraFromDetailBtn) addCameraFromDetailBtn.addEventListener('click', () => {
            if (currentLocationId) openCameraModal(null, currentLocationId);
        });

        // --- Lokasyon Ekle Kaydet ---
        const saveLocationBtn = document.getElementById('saveLocationBtn');
        if (saveLocationBtn) {
            saveLocationBtn.addEventListener('click', async () => {
                const name = document.getElementById('locationNameInput')?.value.trim();
                const desc = document.getElementById('locationDescInput')?.value || '';
                if (!name) return alert('Lokasyon adı zorunludur.');
                try {
                    saveLocationBtn.disabled = true;
                    await CameraService.addLocation(name, desc);
                    showToast('Lokasyon eklendi!', 'success');
                    closeLocationModal();
                    await loadCameraSettingsData();
                } catch (err) {
                    showToast(err.message || 'Lokasyon eklenemedi.', 'error');
                } finally {
                    saveLocationBtn.disabled = false;
                }
            });
        }

        // --- Lokasyon Güncelle Kaydet ---
        const updateLocationBtn = document.getElementById('updateLocationBtn');
        if (updateLocationBtn) {
            updateLocationBtn.addEventListener('click', async () => {
                const id = document.getElementById('editLocationId')?.value;
                const name = document.getElementById('editLocationName')?.value.trim();
                const desc = document.getElementById('editLocationDesc')?.value || '';
                if (!name) return alert('Lokasyon adı zorunludur.');
                try {
                    updateLocationBtn.disabled = true;
                    await CameraService.updateLocation(id, name, desc);
                    showToast('Lokasyon güncellendi!', 'success');
                    closeEditLocationModal();
                    await loadCameraSettingsData();
                } catch (err) {
                    showToast(err.message || 'Lokasyon güncellenemedi.', 'error');
                } finally {
                    updateLocationBtn.disabled = false;
                }
            });
        }

        // --- Kamera Ekle/Güncelle Kaydet ---
        const saveCameraBtn = document.getElementById('saveCameraBtn');
        if (saveCameraBtn) {
            saveCameraBtn.addEventListener('click', async () => {
                const name = document.getElementById('cameraNameInput')?.value.trim();
                const ip = document.getElementById('cameraIpInput')?.value.trim();
                const port = parseInt(document.getElementById('cameraPortInput')?.value, 10) || 80;
                const username = document.getElementById('cameraUsernameInput')?.value || '';
                const password = document.getElementById('cameraPasswordInput')?.value || '';
                if (!name || !ip) return alert('Kamera adı ve IP adresi zorunludur.');

                const cameraData = {
                    locationId: editingCameraLocationId,
                    name,
                    ipAddress: ip,
                    port,
                    username,
                    password,
                    streamChannel: 101
                };

                try {
                    saveCameraBtn.disabled = true;
                    if (editingCameraId) {
                        await CameraService.updateCamera(editingCameraId, cameraData);
                        showToast('Kamera güncellendi!', 'success');
                    } else {
                        await CameraService.addCamera(editingCameraLocationId, cameraData);
                        showToast('Kamera eklendi!', 'success');
                    }
                    closeCameraModal();
                    await loadCameraSettingsData();
                } catch (err) {
                    showToast(err.message || 'Kamera kaydedilemedi.', 'error');
                } finally {
                    saveCameraBtn.disabled = false;
                }
            });
        }

        // --- Modal Kapatma ---
        document.querySelectorAll('#locationModal .close-modal, #locationModal .btn-cancel-modal')
            .forEach(btn => btn.addEventListener('click', closeLocationModal));

        document.querySelectorAll('#editLocationModal .close-modal, #editLocationModal .btn-cancel-modal')
            .forEach(btn => btn.addEventListener('click', closeEditLocationModal));

        document.querySelectorAll('#cameraModal .close-modal-cam, #cameraModal .btn-cancel-modal-cam')
            .forEach(btn => btn.addEventListener('click', closeCameraModal));

        // İlk yükleme
        loadCameraSettingsData();
    }
})();