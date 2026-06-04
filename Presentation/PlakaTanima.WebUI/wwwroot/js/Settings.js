// settings.js - Sadece sidebar geçişi + location ekleme modalı
document.addEventListener('DOMContentLoaded', function() {
    // Sidebar sekme geçişi
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

    // Location ekleme modal
    const globalAddBtn = document.getElementById('globalAddLocationBtn');
    const locationModal = document.getElementById('locationModal');
    const saveLocationBtn = document.getElementById('saveLocationBtn');
    const closeModalBtns = document.querySelectorAll('#locationModal .close-modal, #locationModal .btn-cancel-modal');

    function openLocationModal() {
        if (locationModal) {
            document.getElementById('locationNameInput').value = '';
            document.getElementById('locationDescInput').value = '';
            locationModal.style.display = 'flex';
        }
    }
    function closeLocationModal() {
        if (locationModal) locationModal.style.display = 'none';
    }

    if (globalAddBtn) globalAddBtn.addEventListener('click', openLocationModal);
    if (saveLocationBtn) {
        saveLocationBtn.addEventListener('click', async () => {
            const name = document.getElementById('locationNameInput')?.value.trim();
            const description = document.getElementById('locationDescInput')?.value || '';
            if (!name) return alert('Lokasyon adı gerekli');
            try {
                await window.CameraService.addLocation(name, description);
                alert('Lokasyon eklendi, sayfa yenileniyor...');
                location.reload();
            } catch (err) {
                alert('Hata: ' + err.message);
            }
            closeLocationModal();
        });
    }
    closeModalBtns.forEach(btn => btn.addEventListener('click', closeLocationModal));

    // Diğer butonlar (düzenle, sil, kamera ekle) - ileride yapılacak
    document.getElementById('editLocationBtn')?.addEventListener('click', () => alert('Düzenleme yakında'));
    document.getElementById('deleteLocationBtn')?.addEventListener('click', () => alert('Silme yakında'));
    document.getElementById('addCameraFromDetailBtn')?.addEventListener('click', () => alert('Kamera ekleme yakında'));
});
document.querySelectorAll('#locationTree .tree-node').forEach(node => {
        const caret = node.querySelector('.caret');
        const nested = node.parentElement.querySelector('.nested');
        if (caret && nested) {
            caret.addEventListener('click', (e) => {
                e.stopPropagation();
                nested.classList.toggle('open');
                caret.classList.toggle('open');
            });
        }
        node.addEventListener('click', (e) => {
            // Detay gösterimi için backend'e istek at veya direkt data-id ile işlem yap
            const locId = node.dataset.id;
            // Basitçe ID'yi göster, detayları ileride yaparsın
            document.getElementById('detailLocationName').innerText = node.querySelector('span:last-child').innerText;
            document.getElementById('detailLocationDesc').innerText = 'Açıklama henüz eklenmedi';
            document.getElementById('editLocationBtn').disabled = false;
            document.getElementById('deleteLocationBtn').disabled = false;
            document.getElementById('addCameraFromDetailBtn').disabled = false;
            document.getElementById('detailCameraTableBody').innerHTML = '<tr><td colspan="6">Kamera listesi için tıklayın</td></tr>';
        });
    });