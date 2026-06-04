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

    function showToast(msg, type = 'info') {
        // Basit toast (alert yerine daha iyisi konsol + alert geçici)
        console.log(`[${type.toUpperCase()}] ${msg}`);
        alert(msg);
    }

    if (saveBtn) {
        saveBtn.addEventListener('click', () => {
            const settings = collectStaticSettings();
            console.log('Kaydedilen ayarlar (mock):', settings);
            showToast('Statik ayarlar kaydedildi (mock)', 'success');
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

            const themeCards = document.querySelectorAll('.theme-card');
            themeCards.forEach(c => c.classList.remove('active'));
            const lightTheme = document.querySelector('.theme-card[data-theme="light"]');
            if (lightTheme) lightTheme.classList.add('active');
            if (document.getElementById('darkModeToggle')) document.getElementById('darkModeToggle').checked = false;

            const colorDots = document.querySelectorAll('.color-dot');
            colorDots.forEach(d => d.classList.remove('active'));
            const blueDot = document.querySelector('.color-dot[data-accent="blue"]');
            if (blueDot) blueDot.classList.add('active');

            const densityBtns = document.querySelectorAll('.density-btn');
            densityBtns.forEach(b => b.classList.remove('active'));
            const normalDensity = document.querySelector('.density-btn[data-density="normal"]');
            if (normalDensity) normalDensity.classList.add('active');

            if (document.getElementById('sidebarPosition')) document.getElementById('sidebarPosition').value = 'left';
            if (document.getElementById('animationsEnabled')) document.getElementById('animationsEnabled').checked = true;
            if (document.getElementById('recContinuous')) document.getElementById('recContinuous').checked = false;
            if (document.getElementById('recEvent')) document.getElementById('recEvent').checked = true;
            if (document.getElementById('recRetention')) document.getElementById('recRetention').value = '30';
            if (document.getElementById('recQuality')) document.getElementById('recQuality').value = 'Orta (1080p)';

            showToast('Varsayılan ayarlar yüklendi (kaydetmek için butona tıklayın)', 'info');
        });
    }

    // ---------- DİNAMİK KAMERA-LOKASYON CRUD (MOCK, ANINDA KAYIT) ----------
    let locations = [
        { id: 'loc1', name: 'Ana Giriş', description: 'Ana giriş kapısı', cameras: [
            { id: 'cam1', name: 'Kamera 1', ip: '192.168.1.101', port: 80, username: 'admin', password: '123', status: 'online' },
            { id: 'cam2', name: 'Kamera 2', ip: '192.168.1.102', port: 80, username: 'admin', password: '123', status: 'offline' }
        ] },
        { id: 'loc2', name: 'Otopark', description: 'Açık otopark alanı', cameras: [
            { id: 'cam3', name: 'Otopark Giriş', ip: '192.168.1.201', port: 80, username: 'admin', password: '123', status: 'online' }
        ] }
    ];
    let currentLocationId = null;

    // DOM elemanları (null kontrolü ile)
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
    let editingLocationId = null;
    let editingCameraLocationId = null;
    let editingCameraId = null;

    // Eğer gerekli DOM elemanları yoksa (kamera ayarları paneli dışında) çalışmayı durdur
    if (!locationTree) {
        console.warn('Kamera ayarları DOM elemanları bulunamadı, dinamik kısım atlanıyor.');
    } else {
        // --- Ağaç render ---
        function renderLocationTree(filterText = '') {
            const filter = filterText.toLowerCase();
            const filtered = locations.filter(loc => loc.name.toLowerCase().includes(filter));
            locationTree.innerHTML = '';
            filtered.forEach(loc => {
                const li = document.createElement('li');
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
                    leafLi.innerHTML = `<i class="fas fa-video"></i> ${cam.name}`;
                    leafLi.addEventListener('click', (e) => {
                        e.stopPropagation();
                        alert(`Kamera: ${cam.name}\nIP: ${cam.ip}\nDurum: ${cam.status === 'online' ? 'Çevrimiçi' : 'Çevrimdışı'}`);
                    });
                    ul.appendChild(leafLi);
                });
                li.appendChild(ul);
                nodeDiv.addEventListener('click', (e) => {
                    e.stopPropagation();
                    currentLocationId = loc.id;
                    renderLocationTree(filterText);
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
                    row.insertCell(0).textContent = cam.id;
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
                    btn.addEventListener('click', () => {
                        const camId = btn.dataset.camid;
                        if (confirm('Kamerayı silmek istediğinize emin misiniz?')) {
                            loc.cameras = loc.cameras.filter(c => c.id !== camId);
                            renderLocationTree(locationSearch ? locationSearch.value : '');
                            renderLocationDetail(currentLocationId);
                            showToast('Kamera silindi (mock)', 'info');
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
        function openCameraModal(cam = null, locationId) {
            if (!cameraModal) return;
            editingCameraLocationId = locationId;
            editingCameraId = cam ? cam.id : null;
            const camIdInput = document.getElementById('cameraId');
            const camNameInput = document.getElementById('cameraNameInput');
            const camIpInput = document.getElementById('cameraIpInput');
            const camPortInput = document.getElementById('cameraPortInput');
            const camUserInput = document.getElementById('cameraUsernameInput');
            const camPassInput = document.getElementById('cameraPasswordInput');
            if (camIdInput) camIdInput.value = editingCameraId || '';
            if (camNameInput) camNameInput.value = cam ? cam.name : '';
            if (camIpInput) camIpInput.value = cam ? cam.ip : '';
            if (camPortInput) camPortInput.value = cam ? cam.port : 80;
            if (camUserInput) camUserInput.value = cam ? cam.username : '';
            if (camPassInput) camPassInput.value = cam ? cam.password : '';
            cameraModal.style.display = 'flex';
        }
        function closeCameraModal() { if (cameraModal) cameraModal.style.display = 'none'; }

        if (globalAddLocationBtn) globalAddLocationBtn.addEventListener('click', () => openLocationModal());
        if (editLocationBtn) editLocationBtn.addEventListener('click', () => {
            const loc = locations.find(l => l.id === currentLocationId);
            if (loc) openLocationModal(loc);
        });
        if (deleteLocationBtn) deleteLocationBtn.addEventListener('click', () => {
            if (confirm('Lokasyon ve tüm kameraları silinecek. Devam?')) {
                locations = locations.filter(l => l.id !== currentLocationId);
                currentLocationId = null;
                renderLocationTree(locationSearch ? locationSearch.value : '');
                renderLocationDetail(null);
                showToast('Lokasyon silindi (mock)', 'info');
            }
        });
        if (addCameraFromDetailBtn) addCameraFromDetailBtn.addEventListener('click', () => {
            if (currentLocationId) openCameraModal(null, currentLocationId);
        });

        const saveLocationBtn = document.getElementById('saveLocationBtn');
        if (saveLocationBtn) {
            saveLocationBtn.addEventListener('click', () => {
                const name = document.getElementById('locationNameInput')?.value.trim();
                const desc = document.getElementById('locationDescInput')?.value || '';
                if (!name) return alert('Lokasyon adı gerekli');
                if (editingLocationId) {
                    const loc = locations.find(l => l.id === editingLocationId);
                    if (loc) { loc.name = name; loc.description = desc; }
                } else {
                    const newId = 'loc_' + Date.now();
                    locations.push({ id: newId, name, description: desc, cameras: [] });
                }
                renderLocationTree(locationSearch ? locationSearch.value : '');
                if (currentLocationId) renderLocationDetail(currentLocationId);
                closeLocationModal();
                showToast('Lokasyon kaydedildi (mock)', 'success');
            });
        }
        const closeModalBtns = document.querySelectorAll('#locationModal .close-modal, #locationModal .btn-cancel-modal');
        closeModalBtns.forEach(btn => btn.addEventListener('click', closeLocationModal));

        const saveCameraBtn = document.getElementById('saveCameraBtn');
        if (saveCameraBtn) {
            saveCameraBtn.addEventListener('click', () => {
                const name = document.getElementById('cameraNameInput')?.value.trim();
                const ip = document.getElementById('cameraIpInput')?.value.trim();
                const port = parseInt(document.getElementById('cameraPortInput')?.value, 10);
                const username = document.getElementById('cameraUsernameInput')?.value || '';
                const password = document.getElementById('cameraPasswordInput')?.value || '';
                if (!name || !ip) return alert('Kamera adı ve IP zorunlu');
                const loc = locations.find(l => l.id === editingCameraLocationId);
                if (!loc) return;
                if (editingCameraId) {
                    const cam = loc.cameras.find(c => c.id === editingCameraId);
                    if (cam) { cam.name = name; cam.ip = ip; cam.port = port; cam.username = username; cam.password = password; }
                } else {
                    const newId = 'cam_' + Date.now();
                    loc.cameras.push({ id: newId, name, ip, port, username, password, status: 'offline' });
                }
                renderLocationTree(locationSearch ? locationSearch.value : '');
                if (currentLocationId === editingCameraLocationId) renderLocationDetail(currentLocationId);
                closeCameraModal();
                showToast('Kamera kaydedildi (mock)', 'success');
            });
        }
        const closeCamModalBtns = document.querySelectorAll('#cameraModal .close-modal-cam, #cameraModal .btn-cancel-modal-cam');
        closeCamModalBtns.forEach(btn => btn.addEventListener('click', closeCameraModal));

        if (locationSearch) {
            locationSearch.addEventListener('input', (e) => {
                renderLocationTree(e.target.value);
                if (currentLocationId && !locations.find(l => l.id === currentLocationId && l.name.toLowerCase().includes(e.target.value.toLowerCase()))) {
                    renderLocationDetail(null);
                } else if (currentLocationId) {
                    renderLocationDetail(currentLocationId);
                }
            });
        }

        // İlk yükleme
        renderLocationTree('');
        if (locations.length > 0) {
            currentLocationId = locations[0].id;
            renderLocationTree('');
            renderLocationDetail(currentLocationId);
        } else {
            renderLocationDetail(null);
        }
    }
});