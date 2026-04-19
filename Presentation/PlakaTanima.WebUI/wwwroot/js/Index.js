// --- AĞAÇ YAPISI (TREE VIEW) ---
const toggler = document.getElementsByClassName("tree-node");
for (let i = 0; i < toggler.length; i++) {
    toggler[i].addEventListener("click", function () {
        const nestedList = this.nextElementSibling;
        const caret = this.querySelector('.caret');
        if (nestedList) {
            nestedList.classList.toggle("active-tree");
            caret.classList.toggle("caret-down");
        }
    });
}

// --- GÖRÜNÜM KONTROLLERİ VE HTML ELEMENTLERİ ---
const liveViewContainer = document.getElementById('liveView');
const liveTableContainer = document.getElementById('liveTableContainer'); // Tablo alanı
let slots = document.querySelectorAll('.video-slot');

const btn1View = document.getElementById('btn1View');
const btn4View = document.getElementById('btn4View');
const btn12View = document.getElementById('btn12View');
const btnTableView = document.getElementById('btnTableView'); // Yeni tablo butonu

// Kamerayı slota yükleyen ortak fonksiyon
function loadCameraToSlot(slot, camName, camId) {
    const infoDiv = slot.querySelector('.camera-overlay-info');
    const signalDiv = slot.querySelector('.no-signal');

    const now = new Date();
    const timeString = now.toLocaleTimeString('tr-TR');

    infoDiv.innerHTML = `${timeString} <br> ${camName} (IP: 10.9.99.${camId})`;
    signalDiv.innerHTML = "<i class='fas fa-spinner fa-spin'></i> Bağlanıyor...";
    slot.style.background = "#111";

    setTimeout(() => {
        signalDiv.innerHTML = "";
        slot.style.background = "radial-gradient(circle, #2a4b6b 0%, #111 100%)";
    }, 800);
}

// Aktif slotu belirleme
function setActiveSlot(selectedSlot) {
    slots.forEach(slot => slot.classList.remove('active-slot'));
    selectedSlot.classList.add('active-slot');
}

// Bir slota tıklama ve sürükle-bırak olaylarını ekleyen fonksiyon
function attachSlotEvents(slot) {
    slot.addEventListener('click', function () {
        setActiveSlot(this);
    });

    slot.addEventListener('dragover', function (e) {
        e.preventDefault();
        this.classList.add('drag-over');
    });

    slot.addEventListener('dragleave', function (e) {
        this.classList.remove('drag-over');
    });

    slot.addEventListener('drop', function (e) {
        e.preventDefault();
        this.classList.remove('drag-over');
        setActiveSlot(this);

        const camName = e.dataTransfer.getData('camName');
        const camId = e.dataTransfer.getData('camId');
        if (camName && camId) {
            loadCameraToSlot(this, camName, camId);
        }
    });
}

// Başlangıçta var olan slota olayları tanımla
if (slots.length > 0) {
    attachSlotEvents(slots[0]);
}

// Görünümü ayarlayan ve DOM'dan gereksiz slotları SİLEN/ÜRETEN ana fonksiyon
function setViewMode(gridClass, activeBtn, visibleCount) {
    // 1. Tüm butonların aktifliğini kaldırıp tıklananı aktif yap
    [btn1View, btn4View, btn12View, btnTableView].forEach(btn => {
        if (btn) btn.classList.remove('active-btn');
    });
    if (activeBtn) activeBtn.classList.add('active-btn');

    // 2. Tablo alanını gizle, Kameraları göster
    if (liveTableContainer) liveTableContainer.style.display = 'none';
    if (liveViewContainer) liveViewContainer.style.display = 'grid'; // flex yerine grid kullandım çünkü CSS'te ızgara yapın var

    // 3. FAZLALIKLARI SİL (Örn: 4'lüden 1'liye geçerken 3 tanesini HTML'den siler)
    while (liveViewContainer.children.length > visibleCount) {
        liveViewContainer.removeChild(liveViewContainer.lastChild);
    }

    // 4. EKSİKLERİ YARAT (Örn: 1'liden 4'lüye geçerken 3 tane yeni yaratır)
    while (liveViewContainer.children.length < visibleCount) {
        const nextId = liveViewContainer.children.length + 1;
        const newSlot = document.createElement('div');
        newSlot.className = 'video-slot';
        newSlot.id = `slot-${nextId}`;
        newSlot.innerHTML = `
            <div class="camera-overlay-info"></div>
            <div class="no-signal">Kamera Seçin</div>
        `;
        attachSlotEvents(newSlot);
        liveViewContainer.appendChild(newSlot);
    }

    // 5. Slot listemizi güncelle
    slots = document.querySelectorAll('.video-slot');

    // 6. CSS Grid class'ını güncelle
    liveViewContainer.className = 'live-view-container';
    if (gridClass) liveViewContainer.classList.add(gridClass);
}

// --- BUTON TIKLAMALARI ---
btn1View.addEventListener('click', () => {
    setViewMode(null, btn1View, 1);
    setActiveSlot(slots[0]);
});

btn4View.addEventListener('click', () => {
    setViewMode('grid-4', btn4View, 4);
});

if (btn12View) {
    btn12View.addEventListener('click', () => {
        setViewMode('grid-12', btn12View, 12);
    });
}

// --- YENİ EKLENEN TABLO BUTONUNUN TIKLANMA OLAYI ---
if (btnTableView) {
    btnTableView.addEventListener('click', () => {
        // Tüm butonların aktifliğini kaldırıp tablo butonunu aktif yap
        [btn1View, btn4View, btn12View, btnTableView].forEach(btn => {
            if (btn) btn.classList.remove('active-btn');
        });
        btnTableView.classList.add('active-btn');

        // Kamera div'ini GİZLE, Tablo div'ini GÖSTER
        if (liveViewContainer) liveViewContainer.style.display = 'none';
        if (liveTableContainer) liveTableContainer.style.display = 'block';
    });
}

// --- KAMERA LİSTESİ OLAYLARI (Sürükleme ve Tıklama) ---
const cameraItems = document.querySelectorAll('.camera-item');

cameraItems.forEach(item => {
    item.setAttribute('draggable', 'true');

    item.addEventListener('dragstart', function (e) {
        e.dataTransfer.setData('camName', this.getAttribute('data-name'));
        e.dataTransfer.setData('camId', this.getAttribute('data-id'));
    });

    item.addEventListener('click', function () {
        cameraItems.forEach(c => c.classList.remove('selected'));
        this.classList.add('selected');

        const camName = this.getAttribute('data-name');
        const camId = this.getAttribute('data-id');

        const activeSlot = document.querySelector('.video-slot.active-slot');
        if (!activeSlot) return;

        loadCameraToSlot(activeSlot, camName, camId);
    });
});

// --- SAĞ PANEL AÇ/KAPAT ---
const btnToggleRight = document.getElementById('btnToggleRight');
const rightPanel = document.getElementById('right');

if (btnToggleRight && rightPanel) {
    btnToggleRight.addEventListener('click', () => {
        rightPanel.classList.toggle('hidden');
        btnToggleRight.classList.toggle('active-btn');
    });
}