// ==========================================
// AYARLAR SAYFASI - FULL JS (DÜZELTİLMİŞ)
// ==========================================
document.addEventListener('DOMContentLoaded', function() {
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
    // KAMERA & LOKASYON CRUD (GERÇEK BACKEND API)
    // ==========================================================
    let locations = [];
    let currentLocationId = null;

    // DOM elemanları
    const locationTree = document.getElementById('locationTree');
    const locationSearch = document.getElementById('locationSearchTree');
    const detailLocationName = document.getElementById('detailLocationName');
    const detailLocationDesc = document.getElementById('detailLocationDesc');
    const editLocationBtn = document.getElementById('editLocationBtn');
    const deleteLocationBtn = document.getElementById('deleteLocationBtn');
    const addCameraFromDetailBtn = document.getElementById('addCameraFromDetailBtn');
    const detailCameraTableBody = document.getElementById('detailCameraTableBody');
    const globalAddLocationBtn = document.getElementById('globalAddLocationBtn');

    // Modal elemanları
    const locationModal = document.getElementById('locationModal');
    const cameraModal = document.getElementById('cameraModal');
    const editLocationModal = document.getElementById('editLocationModal');
    let editingLocationId = null;
    let editingCameraLocationId = null;
    let editingCameraId = null;

    if (!locationTree) {
        console.warn('Kamera ayarları DOM elemanları bulunamadı, dinamik kısım atlanıyor.');
    } else {
        // --- Veriyi sunucudan çek ---
        async function loadTreeData() {
            try {
                const res = await fetch('/Camera/GetTree');
                locations = await res.json();
                // Alan adlarını normalize et: server'dan camelCase gelir
                locations = locations.map(loc => ({
                    id: loc.id,
                    name: loc.name,
                    description: loc.description || '',
                    cameras: (loc.cameras || []).map(cam => ({
                        id: cam.id,
                        name: cam.name,
                        ip: cam.ipAddress || '',
                        port: cam.port || 80,
                        username: cam.username || '',
                        password: cam.password || '',
                        status: cam.status || 'offline'
                    }))
                }));
                renderLocationTree(locationSearch ? locationSearch.value : '');
                if (locations.length > 0) {
                    currentLocationId = locations[0].id;
                    renderLocationTree(locationSearch ? locationSearch.value : '');
                    renderLocationDetail(currentLocationId);
                } else {
                    renderLocationDetail(null);
                }
            } catch (err) {
                console.error('Ağaç verisi yüklenemedi:', err);
                showToast('Sunucudan veri alınamadı!', 'error');
            }
        }

        // --- Ağaç render ---
        function renderLocationTree(filterText = '') {
            const filter = filterText.toLowerCase();
            const filtered = locations.filter(loc => loc.name.toLowerCase().includes(filter));
            locationTree.innerHTML = '';
            filtered.forEach(loc => {
                const li = document.createElement('li');
                li.dataset.id = loc.id;
                const nodeDiv = document.createElement('div');
                nodeDiv.className = `tree-node ${currentLocationId === loc.id ? 'active' : ''}`;
                nodeDiv.dataset.id = loc.id;
                const caretSpan = document.createElement('span');
                caretSpan.className = 'caret';
                caretSpan.innerHTML = '&nbsp;';
                nodeDiv.appendChild(caretSpan);
                const icon = document.createElement('i');
                icon.className = 'fas fa-map-marker-alt';
                nodeDiv.appendChild(icon);
                const textSpan = document.createElement('span');
                textSpan.textContent = loc.name;
                nodeDiv.appendChild(textSpan);
                li.appendChild(nodeDiv);
                const ul = document.createElement('ul');
                ul.className = 'nested';
                loc.cameras.forEach(cam => {
                    const leafLi = document.createElement('li');
                    leafLi.className = `camera-leaf ${cam.status}`;
                    leafLi.dataset.camId = cam.id;
                    leafLi.dataset.camName = cam.name;
                    leafLi.dataset.ip = cam.ip;
                    leafLi.dataset.port = cam.port;
                    leafLi.dataset.username = cam.username;
                    leafLi.dataset.password = cam.password;
                    leafLi.dataset.status = cam.status;
                    leafLi.innerHTML = `<i class="fas fa-video"></i> ${cam.name}`;
                    ul.appendChild(leafLi);
                });
                li.appendChild(ul);
                nodeDiv.addEventListener('click', (e) => {
                    e.stopPropagation();
                    currentLocationId = loc.id;
                    renderLocationTree(locationSearch ? locationSearch.value : '');
                    renderLocationDetail(loc.id);
                });
                locationTree.appendChild(li);
            });
            // caret aç/kapa
            document.querySelectorAll('#locationTree .caret').forEach(caret => {
                caret.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const parentLi = caret.closest('li');
                    const nestedUl = parentLi.querySelector('.nested');
                    if (nestedUl) {
                        nestedUl.classList.toggle('open');
                        caret.classList.toggle('open');
                    }
                });
            });
            // İlk lokasyonun ağacını aç
            if (!filterText) {
                const firstCaret = document.querySelector('#locationTree .caret');
                if (firstCaret) {
                    const parentLi = firstCaret.closest('li');
                    const nestedUl = parentLi.querySelector('.nested');
                    if (nestedUl) {
                        nestedUl.classList.add('open');
                        firstCaret.classList.add('open');
                    }
                }
            }
            if (currentLocationId && !locations.find(l => l.id === currentLocationId)) {
                currentLocationId = null;
                renderLocationDetail(null);
            }
        }

        function renderLocationDetail(locationId) {
            if (!locationId) {
                if (detailLocationName) detailLocationName.textContent = 'Lokasyon seçin';
                if (detailLocationDesc) detailLocationDesc.textContent = 'Bir lokasyon seçtiğinizde detaylar burada görünür.';
                if (editLocationBtn) editLocationBtn.disabled = true;
                if (deleteLocationBtn) deleteLocationBtn.disabled = true;
                if (addCameraFromDetailBtn) addCameraFromDetailBtn.disabled = true;
                if (detailCameraTableBody) detailCameraTableBody.innerHTML = '<tr><td colspan="6">Lokasyon seçin</td></tr>';
                return;
            }
            const loc = locations.find(l => l.id === locationId);
            if (!loc) return;
            if (detailLocationName) detailLocationName.textContent = loc.name;
            if (detailLocationDesc) detailLocationDesc.textContent = loc.description || 'Açıklama yok';
            if (editLocationBtn) editLocationBtn.disabled = false;
            if (deleteLocationBtn) deleteLocationBtn.disabled = false;
            if (addCameraFromDetailBtn) addCameraFromDetailBtn.disabled = false;
            if (!detailCameraTableBody) return;
            if (loc.cameras.length === 0) {
                detailCameraTableBody.innerHTML = '<tr><td colspan="6">Bu lokasyonda kamera yok</td></tr>';
            } else {
                detailCameraTableBody.innerHTML = '';
                loc.cameras.forEach(cam => {
                    const row = detailCameraTableBody.insertRow();
                    row.insertCell(0).textContent = cam.id.substring(0, 8) + '...';
                    row.insertCell(1).textContent = cam.name;
                    row.insertCell(2).textContent = cam.ip;
                    row.insertCell(3).textContent = cam.port;
                    row.insertCell(4).innerHTML = `<span class="badge ${cam.status}">${cam.status === 'online' ? 'Çevrimiçi' : 'Çevrimdışı'}</span>`;
                    const actionCell = row.insertCell(5);
                    actionCell.innerHTML = `<button class="action-btn edit" data-camid="${cam.id}"><i class="fas fa-edit"></i></button>
                                            <button class="action-btn delete" data-camid="${cam.id}"><i class="fas fa-trash"></i></button>`;
                });
                // Kamera düzenle/sil eventleri
                document.querySelectorAll('#detailCameraTableBody .action-btn.edit').forEach(btn => {
                    btn.addEventListener('click', () => {
                        const camId = btn.dataset.camid;
                        const camera = loc.cameras.find(c => c.id === camId);
                        if (camera) openCameraModal(camera, locationId);
                    });
                });
                document.querySelectorAll('#detailCameraTableBody .action-btn.delete').forEach(btn => {
                    btn.addEventListener('click', async () => {
                        const camId = btn.dataset.camid;
                        if (confirm('Kamerayı silmek istediğinize emin misiniz?')) {
                            try {
                                await CameraService.deleteCamera(camId);
                                showToast('Kamera silindi!', 'success');
                                await loadTreeData();
                            } catch (err) {
                                showToast(err.message || 'Kamera silinemedi!', 'error');
                            }
                        }
                    });
                });
            }
        }

        // --- Lokasyon modal işlemleri ---
        function openLocationModal(loc = null) {
            if (!locationModal) return;
            editingLocationId = loc ? loc.id : null;
            const locIdInput = document.getElementById('locationId');
            const locNameInput = document.getElementById('locationNameInput');
            const locDescInput = document.getElementById('locationDescInput');
            if (locIdInput) locIdInput.value = editingLocationId || '';
            if (locNameInput) locNameInput.value = loc ? loc.name : '';
            if (locDescInput) locDescInput.value = loc ? (loc.description || '') : '';
            locationModal.style.display = 'flex';
        }
        function closeLocationModal() { if (locationModal) locationModal.style.display = 'none'; }

        function openEditLocationModal(loc = null) {
            if (!editLocationModal) return;
            editingLocationId = loc ? loc.id : null;
            document.getElementById('editLocationId').value = editingLocationId || '';
            document.getElementById('editLocationName').value = loc ? loc.name : '';
            document.getElementById('editLocationDesc').value = loc ? (loc.description || '') : '';
            editLocationModal.style.display = 'flex';
        }
        function closeEditLocationModal() { if (editLocationModal) editLocationModal.style.display = 'none'; }

        function openCameraModal(cam = null, locationId) {
            if (!cameraModal) return;
            editingCameraLocationId = locationId;
            editingCameraId = cam ? cam.id : null;
            document.getElementById('cameraId').value = editingCameraId || '';
            document.getElementById('cameraLocationId').value = editingCameraLocationId || '';
            document.getElementById('cameraNameInput').value = cam ? cam.name : '';
            document.getElementById('cameraIpInput').value = cam ? cam.ip : '';
            document.getElementById('cameraPortInput').value = cam ? cam.port : 80;
            document.getElementById('cameraUsernameInput').value = cam ? cam.username : '';
            document.getElementById('cameraPasswordInput').value = cam ? cam.password : '';
            cameraModal.style.display = 'flex';
        }
        function closeCameraModal() { if (cameraModal) cameraModal.style.display = 'none'; }

        // --- Buton event'leri ---
        if (globalAddLocationBtn) globalAddLocationBtn.addEventListener('click', () => openLocationModal());

        if (editLocationBtn) editLocationBtn.addEventListener('click', () => {
            const loc = locations.find(l => l.id === currentLocationId);
            if (loc) openEditLocationModal(loc);
        });

        if (deleteLocationBtn) {
            deleteLocationBtn.addEventListener('click', async () => {
                if (!currentLocationId) return;
                if (confirm('Lokasyon silinecek. Devam etmek istediğinize emin misiniz?')) {
                    try {
                        await CameraService.deleteLocation(currentLocationId);
                        showToast('Lokasyon silindi!', 'success');
                        currentLocationId = null;
                        await loadTreeData();
                    } catch (err) {
                        showToast(err.message || 'Lokasyon silinemedi!', 'error');
                    }
                }
            });
        }

        if (addCameraFromDetailBtn) addCameraFromDetailBtn.addEventListener('click', () => {
            if (currentLocationId) openCameraModal(null, currentLocationId);
        });

        // --- Lokasyon Ekle (AJAX) ---
        const saveLocationBtn = document.getElementById('saveLocationBtn');
        if (saveLocationBtn) {
            saveLocationBtn.addEventListener('click', async () => {
                const name = document.getElementById('locationNameInput')?.value.trim();
                const desc = document.getElementById('locationDescInput')?.value || '';
                if (!name) return alert('Lokasyon adı gerekli');
                try {
                    await CameraService.addLocation(name, desc);
                    showToast('Lokasyon eklendi!', 'success');
                    closeLocationModal();
                    document.getElementById('locationNameInput').value = '';
                    document.getElementById('locationDescInput').value = '';
                    await loadTreeData();
                } catch (err) {
                    showToast(err.message || 'Lokasyon eklenemedi!', 'error');
                }
            });
        }

        // --- Lokasyon Güncelle (AJAX) ---
        const updateLocationBtn = document.getElementById('updateLocationBtn');
        if (updateLocationBtn) {
            updateLocationBtn.addEventListener('click', async () => {
                const id = document.getElementById('editLocationId')?.value;
                const name = document.getElementById('editLocationName')?.value.trim();
                const desc = document.getElementById('editLocationDesc')?.value || '';
                if (!name) return alert('Lokasyon adı gerekli');
                try {
                    await CameraService.updateLocation(id, name, desc);
                    showToast('Lokasyon güncellendi!', 'success');
                    closeEditLocationModal();
                    await loadTreeData();
                } catch (err) {
                    showToast(err.message || 'Lokasyon güncellenemedi!', 'error');
                }
            });
        }

        // --- Kamera Ekle/Güncelle (AJAX) ---
        const saveCameraBtn = document.getElementById('saveCameraBtn');
        if (saveCameraBtn) {
            saveCameraBtn.addEventListener('click', async () => {
                const name = document.getElementById('cameraNameInput')?.value.trim();
                const ip = document.getElementById('cameraIpInput')?.value.trim();
                const port = parseInt(document.getElementById('cameraPortInput')?.value, 10) || 80;
                const username = document.getElementById('cameraUsernameInput')?.value || '';
                const password = document.getElementById('cameraPasswordInput')?.value || '';
                if (!name || !ip) return alert('Kamera adı ve IP zorunlu');

                const cameraData = {
                    name,
                    ipAddress: ip,
                    port,
                    username,
                    password,
                    streamChannel: 101
                };

                try {
                    if (editingCameraId) {
                        await CameraService.updateCamera(editingCameraId, {
                            id: editingCameraId,
                            locationId: editingCameraLocationId,
                            ...cameraData
                        });
                        showToast('Kamera güncellendi!', 'success');
                    } else {
                        await CameraService.addCamera(editingCameraLocationId, cameraData);
                        showToast('Kamera eklendi!', 'success');
                    }
                    closeCameraModal();
                    await loadTreeData();
                } catch (err) {
                    showToast(err.message || 'Kamera kaydedilemedi!', 'error');
                }
            });
        }

        // --- Modal kapatma butonları ---
        document.querySelectorAll('#locationModal .close-modal, #locationModal .btn-cancel-modal')
            .forEach(btn => btn.addEventListener('click', closeLocationModal));

        document.querySelectorAll('#editLocationModal .close-modal, #editLocationModal .btn-cancel-modal')
            .forEach(btn => btn.addEventListener('click', closeEditLocationModal));

        document.querySelectorAll('#cameraModal .close-modal-cam, #cameraModal .btn-cancel-modal-cam')
            .forEach(btn => btn.addEventListener('click', closeCameraModal));

        // --- Arama ---
        if (locationSearch) {
            locationSearch.addEventListener('input', (e) => {
                renderLocationTree(e.target.value);
                if (currentLocationId) {
                    const found = locations.find(l => l.id === currentLocationId && l.name.toLowerCase().includes(e.target.value.toLowerCase()));
                    if (!found) renderLocationDetail(null);
                    else renderLocationDetail(currentLocationId);
                }
            });
        }

        // İlk yükleme - sunucudan veri çek
        loadTreeData();
    }
});