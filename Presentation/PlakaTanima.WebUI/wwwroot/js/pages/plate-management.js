// ==========================================
// PLAKA YÖNETİMİ - Full JS (Dinamik Entegrasyon)
// ==========================================
(function () {
    if (!document.getElementById('pyTableBody')) return;

    let vehicles = [];
    let currentFilter = 'all';
    let currentSearch = '';

    // ==========================================================
    // 1. DOM REFERANSLARI
    // ==========================================================
    const menuItems = document.querySelectorAll('.py-menu-item');
    const filterBtns = document.querySelectorAll('.py-filter-btn');
    const tableBody = document.getElementById('pyTableBody');
    const tableEmpty = document.getElementById('pyTableEmpty');
    const searchInput = document.getElementById('pySearchInput');
    const addBtn = document.getElementById('pyAddBtn');

    // Modal
    const modalOverlay = document.getElementById('pyModalOverlay');
    const modalClose = document.getElementById('pyModalClose');
    const modalCancel = document.getElementById('pyModalCancel');
    const modalSave = document.getElementById('pyModalSave');
    const modalTitle = document.getElementById('pyModalTitle');
    const editId = document.getElementById('pyEditId');
    const formPlate = document.getElementById('pyFormPlate');
    const formModel = document.getElementById('pyFormModel');
    const formOwner = document.getElementById('pyFormOwner');
    const formCategory = document.getElementById('pyFormCategory');
    const formNote = document.getElementById('pyFormNote');

    // Sil modalı
    const deleteOverlay = document.getElementById('pyDeleteOverlay');
    const deleteClose = document.getElementById('pyDeleteClose');
    const deleteCancel = document.getElementById('pyDeleteCancel');
    const deleteConfirm = document.getElementById('pyDeleteConfirm');
    const deletePlate = document.getElementById('pyDeletePlate');
    let deleteTargetId = null;

    // ==========================================================
    // 2. YARDIMCI FONKSİYONLAR
    // ==========================================================
    function getCategoryBadge(category) {
        const labels = {
            normal: 'Normal',
            vip: 'VIP',
            blacklist: 'Kara Liste',
            staff: 'Personel'
        };
        const cls = {
            normal: 'py-badge-normal',
            vip: 'py-badge-vip',
            blacklist: 'py-badge-blacklist',
            staff: 'py-badge-staff'
        };
        const icons = {
            normal: 'fa-check-circle',
            vip: 'fa-star',
            blacklist: 'fa-ban',
            staff: 'fa-id-badge'
        };
        return `<span class="py-badge ${cls[category] || 'py-badge-normal'}"><i class="fas ${icons[category] || 'fa-check-circle'}"></i> ${labels[category] || category}</span>`;
    }

    // ==========================================================
    // 3. API SERVIS ÇAĞRILARI
    // ==========================================================
    async function loadVehicles() {
        try {
            const res = await fetch('/Vehicle/GetVehiclesApi');
            const data = await res.json();
            if (Array.isArray(data)) {
                vehicles = data;
            } else if (data.success === false) {
                showToast(data.message || 'Veriler yüklenemedi.', 'error');
            }
        } catch (err) {
            console.error('Veri çekme hatası:', err);
            showToast('Sunucu bağlantı hatası.', 'error');
        }
        renderTable();
    }

    // ==========================================================
    // 4. TABLO RENDER
    // ==========================================================
    function renderTable() {
        let filtered = [...vehicles];

        // Kategori filtresi (sidebar)
        if (currentFilter !== 'all') {
            filtered = filtered.filter(v => v.category === currentFilter);
        }

        // Arama
        if (currentSearch.trim()) {
            const q = currentSearch.toLowerCase().trim();
            filtered = filtered.filter(v =>
                v.plate.toLowerCase().includes(q) ||
                v.model.toLowerCase().includes(q) ||
                v.owner.toLowerCase().includes(q)
            );
        }

        // İstatistikleri güncelle
        updateStats();

        // Boş tablo kontrolü
        if (filtered.length === 0) {
            tableBody.innerHTML = '';
            tableEmpty.style.display = 'flex';
            return;
        }
        tableEmpty.style.display = 'none';

        // Satırları oluştur
        tableBody.innerHTML = filtered.map(v => `
            <tr>
                <td><span style="font-weight: 600; color: var(--text-primary); font-family: 'SF Mono', 'Consolas', monospace;">${v.plate}</span></td>
                <td>${v.model}</td>
                <td>${v.owner}</td>
                <td>${getCategoryBadge(v.category)}</td>
                <td style="color: var(--text-secondary); font-size: 0.85rem;">${v.date}</td>
                <td>
                    <div class="py-action-group">
                        <button class="py-action-btn" data-action="view" data-id="${v.id}" title="Görüntüle"><i class="fas fa-eye"></i></button>
                        <button class="py-action-btn" data-action="edit" data-id="${v.id}" title="Düzenle"><i class="fas fa-edit"></i></button>
                        <button class="py-action-btn danger" data-action="delete" data-id="${v.id}" title="Sil"><i class="fas fa-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');

        // Buton event'lerini bağla
        document.querySelectorAll('.py-action-btn').forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.stopPropagation();
                const action = this.dataset.action;
                const id = this.dataset.id;
                if (action === 'view') viewVehicle(id);
                else if (action === 'edit') editVehicle(id);
                else if (action === 'delete') confirmDelete(id);
            });
        });
    }

    // ==========================================================
    // 5. İSTATİSTİK GÜNCELLEME
    // ==========================================================
    function updateStats() {
        document.getElementById('statTotal').textContent = vehicles.length;
        document.getElementById('statVip').textContent = vehicles.filter(v => v.category === 'vip').length;
        document.getElementById('statBlacklist').textContent = vehicles.filter(v => v.category === 'blacklist').length;
        document.getElementById('statStaff').textContent = vehicles.filter(v => v.category === 'staff').length;
    }

    // ==========================================================
    // 6. SIDEBAR / FİLTRE GEÇİŞİ
    // ==========================================================
    function switchTab(tabId) {
        currentFilter = tabId;

        // Sidebar
        menuItems.forEach(m => m.classList.remove('active'));
        const activeMenuItem = document.querySelector(`.py-menu-item[data-tab="${tabId}"]`);
        if (activeMenuItem) activeMenuItem.classList.add('active');

        // Filter butonları
        filterBtns.forEach(b => b.classList.remove('active'));
        const activeFilter = document.querySelector(`.py-filter-btn[data-filter="${tabId}"]`);
        if (activeFilter) activeFilter.classList.add('active');

        renderTable();
    }

    menuItems.forEach(item => {
        item.addEventListener('click', () => switchTab(item.dataset.tab));
    });

    filterBtns.forEach(btn => {
        btn.addEventListener('click', () => switchTab(btn.dataset.filter));
    });

    // ==========================================================
    // 7. ARAMA
    // ==========================================================
    searchInput.addEventListener('input', function () {
        currentSearch = this.value;
        renderTable();
    });

    // ==========================================================
    // 8. MODAL YÖNETİMİ
    // ==========================================================
    function openModal(title, vehicle = null) {
        modalTitle.textContent = title;
        if (vehicle) {
            editId.value = vehicle.id;
            formPlate.value = vehicle.plate;
            formModel.value = vehicle.model;
            formOwner.value = vehicle.owner;
            formCategory.value = vehicle.category;
            formNote.value = vehicle.note || '';
        } else {
            editId.value = '';
            formPlate.value = '';
            formModel.value = '';
            formOwner.value = '';
            formCategory.value = 'normal';
            formNote.value = '';
        }
        modalOverlay.classList.remove('hidden');
        formPlate.focus();
    }

    function closeModal() {
        modalOverlay.classList.add('hidden');
    }

    async function saveVehicle() {
        const plate = formPlate.value.trim();
        const model = formModel.value.trim();
        const owner = formOwner.value.trim();
        const category = formCategory.value;
        const note = formNote.value.trim();

        // Validasyon
        if (!plate) { showToast('Plaka zorunludur!', 'error'); formPlate.focus(); return; }
        if (!model) { showToast('Marka/Model zorunludur!', 'error'); formModel.focus(); return; }
        if (!owner) { showToast('Sahip/Sürücü zorunludur!', 'error'); formOwner.focus(); return; }

        const editIdVal = editId.value;
        const payload = {
            plate,
            model,
            owner,
            category,
            note
        };

        try {
            modalSave.disabled = true;
            let url = '/Vehicle/AddVehicleApi';
            if (editIdVal) {
                url = '/Vehicle/UpdateVehicleApi';
                payload.id = editIdVal;
            }

            const res = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });
            const result = await res.json();

            if (result.success) {
                showToast(result.message || 'Araç başarıyla kaydedildi!', 'success');
                closeModal();
                await loadVehicles();
            } else {
                showToast(result.message || 'Kayıt başarısız oldu.', 'error');
            }
        } catch (err) {
            console.error('Kayıt hatası:', err);
            showToast('Sunucuyla iletişim kurulurken hata oluştu.', 'error');
        } finally {
            modalSave.disabled = false;
        }
    }

    addBtn.addEventListener('click', () => openModal('Yeni Araç Ekle'));
    modalClose.addEventListener('click', closeModal);
    modalCancel.addEventListener('click', closeModal);
    modalSave.addEventListener('click', saveVehicle);

    // Modal dışına tıklayınca kapat
    modalOverlay.addEventListener('click', function (e) {
        if (e.target === this) closeModal();
    });

    // Enter ile kaydet
    document.querySelectorAll('.py-form-input, .py-form-select').forEach(el => {
        el.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') saveVehicle();
        });
    });

    // ==========================================================
    // 9. GÖRÜNTÜLE
    // ==========================================================
    function viewVehicle(id) {
        const v = vehicles.find(v => v.id === id);
        if (!v) return;
        window.open(`/VehicleDetails?plate=${encodeURIComponent(v.plate)}`, '_blank');
    }

    // ==========================================================
    // 10. DÜZENLE
    // ==========================================================
    function editVehicle(id) {
        const v = vehicles.find(v => v.id === id);
        if (!v) return;
        openModal('Araç Düzenle', v);
    }

    // ==========================================================
    // 11. SİL
    // ==========================================================
    function confirmDelete(id) {
        const v = vehicles.find(v => v.id === id);
        if (!v) return;
        deleteTargetId = id;
        deletePlate.textContent = v.plate;
        deleteOverlay.classList.remove('hidden');
    }

    function closeDeleteModal() {
        deleteOverlay.classList.add('hidden');
        deleteTargetId = null;
    }

    async function executeDelete() {
        if (!deleteTargetId) return;
        try {
            deleteConfirm.disabled = true;
            const res = await fetch(`/Vehicle/DeleteVehicleApi?id=${deleteTargetId}`, {
                method: 'POST'
            });
            const result = await res.json();
            if (result.success) {
                showToast(result.message || 'Araç başarıyla silindi!', 'success');
                closeDeleteModal();
                await loadVehicles();
            } else {
                showToast(result.message || 'Silme işlemi başarısız oldu.', 'error');
            }
        } catch (err) {
            console.error('Silme hatası:', err);
            showToast('Sunucuyla iletişim kurulurken hata oluştu.', 'error');
        } finally {
            deleteConfirm.disabled = false;
        }
    }

    deleteClose.addEventListener('click', closeDeleteModal);
    deleteCancel.addEventListener('click', closeDeleteModal);
    deleteConfirm.addEventListener('click', executeDelete);
    deleteOverlay.addEventListener('click', function (e) {
        if (e.target === this) closeDeleteModal();
    });

    // ==========================================================
    // 12. TOAST BİLDİRİMİ
    // ==========================================================
    function showToast(msg, type) {
        // Mevcut toast'ları temizle
        document.querySelectorAll('.py-toast').forEach(t => t.remove());

        const toast = document.createElement('div');
        toast.className = `py-toast py-toast-${type || 'info'}`;
        toast.innerHTML = msg;
        document.body.appendChild(toast);

        // Stil ekle (dinamik)
        toast.style.cssText = `
            position: fixed; bottom: 24px; right: 24px; z-index: 9999;
            padding: 14px 24px; border-radius: 10px; font-size: 0.9rem;
            font-weight: 500; font-family: 'Inter', sans-serif;
            box-shadow: 0 8px 24px rgba(0,0,0,0.2);
            animation: pyToastIn 0.3s ease;
            background: ${type === 'error' ? 'var(--danger)' : 'var(--success)'};
            color: white;
            border: 1px solid ${type === 'error' ? 'var(--danger-dark)' : 'var(--success-dark)'};
        `;

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateY(10px)';
            toast.style.transition = 'all 0.3s ease';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    // Toast animasyonu
    const style = document.createElement('style');
    style.textContent = `
        @keyframes pyToastIn {
            from { opacity: 0; transform: translateY(20px); }
            to { opacity: 1; transform: translateY(0); }
        }
    `;
    document.head.appendChild(style);

    // ==========================================================
    // 13. BAŞLANGIÇ
    // ==========================================================
    loadVehicles();
})();
