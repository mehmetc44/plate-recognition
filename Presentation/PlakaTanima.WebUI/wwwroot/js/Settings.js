// settings.js - Sadece UI (sidebar geçişi, modal, ağaç, arama, tıklama ile sağ paneli doldurma)
document.addEventListener('DOMContentLoaded', function () {
    // ---------- SIDEBAR GEÇİŞİ ----------
    const menuItems = document.querySelectorAll('.menu-item');
    const panels = document.querySelectorAll('.settings-panel');
    function switchTab(tabId) {
        panels.forEach(p => p.classList.remove('active'));
        const activePanel = document.getElementById(`panel-${tabId}`);
        if (activePanel) activePanel.classList.add('active');
        menuItems.forEach(m => m.classList.remove('active'));
        const activeMenuItem = document.querySelector(`.menu-item[data-tab="${tabId}"]`);
        if (activeMenuItem) activeMenuItem.classList.add('active');
    }
    menuItems.forEach(item => {
        item.addEventListener('click', () => switchTab(item.getAttribute('data-tab')));
    });

    // ---------- KAMERA AYARLARI (AĞAÇ, ARAMA, SAĞ PANEL) ----------
    const locationTree = document.getElementById('locationTree');
    if (!locationTree) return;

    // Ağaç içinde caret aç/kapa
    function attachCaretEvents() {
        document.querySelectorAll('#locationTree .caret').forEach(caret => {
            caret.removeEventListener('click', caretHandler);
            caret.addEventListener('click', caretHandler);
        });
    }
    function caretHandler(e) {
        e.stopPropagation();
        const li = this.closest('li');
        const nested = li?.querySelector('.nested');
        if (nested) {
            nested.classList.toggle('open');
            this.classList.toggle('open');
        }
    }

    // Arama filtresi
    const searchInput = document.getElementById('locationSearchTree');
    if (searchInput) {
        searchInput.addEventListener('input', function (e) {
            const filter = e.target.value.toLowerCase();
            document.querySelectorAll('#locationTree > li').forEach(li => {
                const name = li.getAttribute('data-name')?.toLowerCase() || '';
                li.style.display = name.includes(filter) ? '' : 'none';
            });
        });
    }

    // Sağ panel elemanları
    const detailName = document.getElementById('detailLocationName');
    const detailDesc = document.getElementById('detailLocationDesc');
    const editLocationBtn = document.getElementById('editLocationBtn');
    const deleteLocationBtn = document.getElementById('deleteLocationBtn');
    const addCameraBtn = document.getElementById('addCameraFromDetailBtn');
    const cameraTableBody = document.getElementById('detailCameraTableBody');









    // Lokasyon tıklama: sağ paneli doldur (sadece görsel, AJAX yok)
    function onLocationClick(node) {
        const li = node.closest('li');
        if (!li) return;
        const locationId = li.getAttribute('data-id');
        const name = li.getAttribute('data-name') || 'Lokasyon seçin';
        const desc = li.getAttribute('data-description') || 'Açıklama yok';

        detailName.innerText = name;
        detailDesc.innerText = desc;
        editLocationBtn.disabled = false;
        deleteLocationBtn.disabled = false;
        addCameraBtn.disabled = false;
        cameraTableBody.innerHTML = '<tr><td colspan="6">Bu lokasyona ait kameralar yakında listelenecek</td></tr>';

        // Sil butonunun onclick olayını temizle ve yeniden ata
        deleteLocationBtn.onclick = null;
        deleteLocationBtn.onclick = async () => {
            if (!confirm('Bu lokasyonu silmek istediğinize emin misiniz?')) return;
            try {
                await window.CameraService.deleteLocation(locationId);
                // Silme başarılı: li'yi kaldır
                li.remove();
                // Sağ paneli sıfırla
                detailName.innerText = 'Lokasyon seçin';
                detailDesc.innerText = 'Bir lokasyon seçin';
                editLocationBtn.disabled = true;
                deleteLocationBtn.disabled = true;
                addCameraBtn.disabled = true;
                cameraTableBody.innerHTML = '<tr><td colspan="6">Lokasyon seçin</td></tr>';
            } catch (err) {
                alert('Silme hatası: ' + err.message);
            }
        };
    }
    // settings.js - Lokasyon ekleme işlemi (modal kaydet butonu)
    const saveLocationBtn = document.getElementById('saveLocationBtn');
    if (saveLocationBtn) {
        saveLocationBtn.onclick = async () => {
            const nameInput = document.getElementById('locationNameInput');
            const descInput = document.getElementById('locationDescInput');
            const name = nameInput?.value.trim();
            const description = descInput?.value.trim() || '';
            if (!name) {
                alert('Lokasyon adı gerekli');
                return;
            }
            try {
                const newId = await window.CameraService.addLocation(name, description);
                // Yeni lokasyonu ağaca ekle
                const tree = document.getElementById('locationTree');
                const newLi = document.createElement('li');
                newLi.setAttribute('data-id', newId);
                newLi.setAttribute('data-name', name);
                newLi.setAttribute('data-description', description);
                newLi.innerHTML = `
                <div class="tree-node" data-id="${newId}">
                    <span class="caret"></span>
                    <i class="fas fa-map-marker-alt"></i>
                    <span>${escapeHtml(name)}</span>
                </div>
                <ul class="nested">
                    <li class="camera-leaf empty">Kamera yok</li>
                </ul>
            `;
                tree.appendChild(newLi);
                // Yeni eklenen düğüme tıklama olayını bağla
                const newNode = newLi.querySelector('.tree-node');
                if (newNode) {
                    newNode.addEventListener('click', function (e) {
                        e.stopPropagation();
                        onLocationClick(this);
                        document.querySelectorAll('#locationTree .tree-node').forEach(n => n.classList.remove('active'));
                        this.classList.add('active');
                    });
                }
                // Yeni caret için aç/kapa olayını bağla
                const newCaret = newLi.querySelector('.caret');
                if (newCaret) {
                    newCaret.addEventListener('click', function (e) {
                        e.stopPropagation();
                        const li = this.closest('li');
                        const nested = li.querySelector('.nested');
                        if (nested) {
                            nested.classList.toggle('open');
                            this.classList.toggle('open');
                        }
                    });
                }
                // Modal'ı kapat ve inputları temizle
                const modal = document.getElementById('locationModal');
                if (modal) modal.style.display = 'none';
                nameInput.value = '';
                descInput.value = '';
                // Opsiyonel: yeni eklenen lokasyonu seçili yap
                newNode.click();
            } catch (err) {
                alert('Ekleme hatası: ' + err.message);
            }
        };
    }
    const updateBtn = document.getElementById('updateLocationBtn');
    if (updateBtn) {
        updateBtn.onclick = async () => {
            const id = document.getElementById('editLocationId').value;
            const name = document.getElementById('editLocationName').value.trim();
            const description = document.getElementById('editLocationDesc').value.trim();
            if (!name) {
                alert('Lokasyon adı gerekli');
                return;
            }
            try {
                await window.CameraService.updateLocation(id, name, description);
                // Ağaçtaki li'yi güncelle
                const li = document.querySelector(`#locationTree li[data-id="${id}"]`);
                if (li) {
                    li.setAttribute('data-name', name);
                    li.setAttribute('data-description', description);
                    const nodeDiv = li.querySelector('.tree-node span:last-child');
                    if (nodeDiv) nodeDiv.textContent = name;
                }
                // Sağ paneli güncelle (eğer aktifse)
                const activeNode = document.querySelector('#locationTree .tree-node.active');
                if (activeNode && activeNode.closest('li').getAttribute('data-id') === id) {
                    document.getElementById('detailLocationName').innerText = name;
                    document.getElementById('detailLocationDesc').innerText = description || 'Açıklama yok';
                }
                // Modalı kapat
                document.getElementById('editLocationModal').style.display = 'none';
                alert('Lokasyon güncellendi');
            } catch (err) {
                alert('Güncelleme hatası: ' + err.message);
            }
        };
    }
    // escapeHtml yardımcı fonksiyonu (eğer yoksa)
    function escapeHtml(str) {
        if (!str) return '';
        return str.replace(/[&<>]/g, function (m) {
            if (m === '&') return '&amp;';
            if (m === '<') return '&lt;';
            if (m === '>') return '&gt;';
            return m;
        });
    }


// Kamera ekle butonu
if (addCameraBtn) {
    addCameraBtn.onclick = () => {
        const activeLi = document.querySelector('#locationTree .tree-node.active')?.closest('li');
        if (!activeLi) {
            alert('Önce bir lokasyon seçin');
            return;
        }
        const locationId = activeLi.getAttribute('data-id');
        document.getElementById('cameraLocationId').value = locationId;  // gizli input
        document.getElementById('cameraNameInput').value = '';
        document.getElementById('cameraIpInput').value = '';
        document.getElementById('cameraPortInput').value = '80';
        document.getElementById('cameraUsernameInput').value = '';
        document.getElementById('cameraPasswordInput').value = '';
        document.getElementById('cameraModal').style.display = 'flex';
    };
}
// ----- KAMERA EKLEME -----
const cameraModal = document.getElementById('cameraModal');
const saveCameraBtn = document.getElementById('saveCameraBtn');

// Kamera ekle butonuna tıklama (modal'ı aç ve lokasyon ID'sini ayarla)
// ----- KAMERA EKLEME -----

if (addCameraBtn) {
    addCameraBtn.onclick = () => {
        const activeLi = document.querySelector('#locationTree .tree-node.active')?.closest('li');
        if (!activeLi) {
            alert('Önce bir lokasyon seçin');
            return;
        }
        const locationId = activeLi.getAttribute('data-id');
        
        // Input elemanlarını kontrol et (cameraModal içinde olmalı)
        const cameraLocationInput = document.getElementById('cameraLocationId');
        const cameraIdInput = document.getElementById('cameraId');
        const nameInput = document.getElementById('cameraNameInput');
        const ipInput = document.getElementById('cameraIpInput');
        const portInput = document.getElementById('cameraPortInput');
        const usernameInput = document.getElementById('cameraUsernameInput');
        const passwordInput = document.getElementById('cameraPasswordInput');
        
        if (!cameraLocationInput || !cameraIdInput) {
            alert('Kamera modalı eksik (gerekli inputlar bulunamadı). Lütfen HTML\'e cameraModal içine cameraLocationId ve cameraId inputlarını ekleyin.');
            return;
        }
        
        cameraLocationInput.value = locationId;
        if (cameraIdInput) cameraIdInput.value = '';
        if (nameInput) nameInput.value = '';
        if (ipInput) ipInput.value = '';
        if (portInput) portInput.value = '80';
        if (usernameInput) usernameInput.value = '';
        if (passwordInput) passwordInput.value = '';
        
        if (cameraModal) cameraModal.style.display = 'flex';
    };
}

// Kamera kaydet butonu (AJAX ile ekle)
if (saveCameraBtn) {
    saveCameraBtn.onclick = async () => {
        const locationId = document.getElementById('cameraLocationId').value;
        const name = document.getElementById('cameraNameInput').value.trim();
        const ip = document.getElementById('cameraIpInput').value.trim();
        const port = parseInt(document.getElementById('cameraPortInput').value, 10);
        const username = document.getElementById('cameraUsernameInput').value.trim();
        const password = document.getElementById('cameraPasswordInput').value;

        if (!name || !ip) {
            alert('Kamera adı ve IP adresi zorunludur');
            return;
        }

        try {
            const newCameraId = await window.CameraService.addCamera(locationId, {
                name,
                ipAddress: ip,
                port,
                username,
                password,
                status: 'Offline'
            });

            // 1) Ağaç yapısını güncelle
            const li = document.querySelector(`#locationTree li[data-id="${locationId}"]`);
            const nestedUl = li?.querySelector('.nested');
            if (nestedUl) {
                // "Kamera yok" yazısını kaldır
                const emptyLi = nestedUl.querySelector('.camera-leaf.empty');
                if (emptyLi) emptyLi.remove();

                // Yeni kamera elemanını ekle
                const newCamLi = document.createElement('li');
                newCamLi.className = 'camera-leaf offline';
                newCamLi.setAttribute('data-cam-id', newCameraId);
                newCamLi.innerHTML = `<i class="fas fa-video"></i> ${escapeHtml(name)}`;
                nestedUl.appendChild(newCamLi);
            }

            // 2) Sağ paneldeki tabloyu güncelle (eğer seçili lokasyon bu ise)
            const activeNode = document.querySelector('#locationTree .tree-node.active');
            if (activeNode && activeNode.closest('li').getAttribute('data-id') === locationId) {
                const tbody = document.getElementById('detailCameraTableBody');
                // Eğer tabloda "Bu lokasyona ait kameralar yakında listelenecek" veya boş mesaj varsa, tabloyu sıfırla
                if (tbody.innerHTML.includes('Bu lokasyona ait kameralar yakında listelenecek') ||
                    tbody.innerHTML.includes('Lokasyon seçin') ||
                    tbody.innerHTML.includes('Bu lokasyonda kamera yok')) {
                    tbody.innerHTML = ''; // temizle, baştan doldurulacak
                }
                // Yeni satır ekle
                const row = tbody.insertRow();
                row.insertCell(0).textContent = newCameraId.substring(0, 8);
                row.insertCell(1).textContent = name;
                row.insertCell(2).textContent = ip;
                row.insertCell(3).textContent = port;
                row.insertCell(4).innerHTML = '<span class="badge offline">Çevrimdışı</span>';
                row.insertCell(5).innerHTML = `
                    <button class="action-btn edit-camera" data-camid="${newCameraId}"><i class="fas fa-edit"></i></button>
                    <button class="action-btn delete-camera" data-camid="${newCameraId}"><i class="fas fa-trash"></i></button>
                `;
            }

            // Modal'ı kapat
            cameraModal.style.display = 'none';
            alert('Kamera başarıyla eklendi');
        } catch (err) {
            alert('Kamera eklenemedi: ' + err.message);
        }
    };
}

// Modal kapatma butonları
document.querySelectorAll('#cameraModal .close-modal-cam, #cameraModal .btn-cancel-modal-cam').forEach(btn => {
    btn.addEventListener('click', () => {
        cameraModal.style.display = 'none';
    });
});

// escapeHtml yardımcı fonksiyonu (eğer yoksa)
function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/[&<>]/g, function(m) {
        if (m === '&') return '&amp;';
        if (m === '<') return '&lt;';
        if (m === '>') return '&gt;';
        return m;
    });
}



















    editLocationBtn.onclick = () => {
        const li = document.querySelector('#locationTree .tree-node.active')?.closest('li');
        if (!li) return;
        const id = li.getAttribute('data-id');
        const name = li.getAttribute('data-name');
        const desc = li.getAttribute('data-description') || '';
        document.getElementById('editLocationId').value = id;
        document.getElementById('editLocationName').value = name;
        document.getElementById('editLocationDesc').value = desc;
        document.getElementById('editLocationModal').style.display = 'flex';
    };// Kapatma butonları
    document.querySelectorAll('#editLocationModal .close-edit-modal, #editLocationModal .btn-cancel-edit-modal').forEach(btn => {
        btn.addEventListener('click', () => {
            document.getElementById('editLocationModal').style.display = 'none';
        });
    });
    // Modal dışına tıklayınca kapat
    window.addEventListener('click', (e) => {
        const modal = document.getElementById('editLocationModal');
        if (e.target === modal) modal.style.display = 'none';
    });
    // Tree node tıklama olaylarını bağla
    function attachTreeNodeEvents() {
        document.querySelectorAll('#locationTree .tree-node').forEach(node => {
            node.removeEventListener('click', nodeClickHandler);
            node.addEventListener('click', nodeClickHandler);
        });
    }
    function nodeClickHandler(e) {
        e.stopPropagation();
        onLocationClick(this);
        // Aktif sınıfını güncelle
        document.querySelectorAll('#locationTree .tree-node').forEach(n => n.classList.remove('active'));
        this.classList.add('active');
    }

    // İlk bağlamalar
    attachCaretEvents();
    attachTreeNodeEvents();

    // ---------- MODAL AÇ/KAPA (LOKASYON EKLEME) ----------
    const addLocationBtn = document.getElementById('globalAddLocationBtn');
    const locationModal = document.getElementById('locationModal');
    const closeModalBtns = document.querySelectorAll('#locationModal .close-modal, #locationModal .btn-cancel-modal');
    if (addLocationBtn && locationModal) {
        addLocationBtn.onclick = () => locationModal.style.display = 'flex';
        closeModalBtns.forEach(btn => btn.onclick = () => locationModal.style.display = 'none');
        window.onclick = (e) => { if (e.target === locationModal) locationModal.style.display = 'none'; };
    }
});