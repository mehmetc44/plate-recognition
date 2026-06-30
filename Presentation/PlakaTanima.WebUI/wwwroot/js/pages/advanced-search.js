// ==============================================================================
// Platar - Gelişmiş Sorgu JS (AJAX Entegrasyonu ve Dinamik Arayüz)
// ==============================================================================
(function () {
    let allEvents = []; // Filtrelenmemiş tüm olaylar (Geçiş geçmişi için kullanılacak)
    let filters = {
        search: "",
        type: [],
        direction: null,
        time: null,
        range: { start: null, end: null }
    };

    // DOM Referansları
    const tableBody = document.querySelector(".query-table tbody");
    const resultCountEl = document.getElementById("resultCount");
    const detailPanel = document.getElementById("detailPanel");
    const searchInput = document.querySelector(".search-box input");
    
    // Arama Girişi Event Listener'ı
    if (searchInput) {
        // Arama kutusuna id veriyoruz dinamik olarak
        searchInput.id = "querySearchInput";
        searchInput.addEventListener("input", function () {
            filters.search = this.value;
            updateResults();
        });
    }

    // Arama butonu ve temizle butonu dinleyicileri
    const btnFilter = document.querySelector(".btn-filter");
    const btnClear = document.querySelector(".btn-clear");
    if (btnFilter) {
        btnFilter.addEventListener("click", updateResults);
    }
    if (btnClear) {
        btnClear.addEventListener("click", resetFilters);
    }

    // ==========================================================
    // 1. DİNAMİK VERİ ÇEKME & TABLO GÜNCELLEME
    // ==========================================================
    async function updateResults() {
        if (!tableBody) return;

        // Yükleniyor durumu göster
        tableBody.innerHTML = `
            <tr>
                <td colspan="6" style="text-align: center; padding: 40px; color: var(--text-secondary);">
                    <i class="fas fa-spinner fa-spin" style="font-size: 1.5rem; margin-bottom: 10px;"></i>
                    <p>Veriler sorgulanıyor, lütfen bekleyin...</p>
                </td>
            </tr>`;

        // URL Parametrelerini oluştur
        const url = new URL('/api/lpr/query', window.location.origin);
        if (filters.search) url.searchParams.append('search', filters.search);
        if (filters.type.length > 0) url.searchParams.append('categories', filters.type.join(','));
        if (filters.direction) url.searchParams.append('direction', filters.direction);
        if (filters.range.start) url.searchParams.append('startDate', filters.range.start);
        if (filters.range.end) url.searchParams.append('endDate', filters.range.end);

        try {
            const res = await fetch(url);
            const data = await res.json();

            if (Array.isArray(data)) {
                allEvents = data;
                renderTable(data);
            } else {
                tableBody.innerHTML = `
                    <tr>
                        <td colspan="6" style="text-align: center; padding: 40px; color: var(--danger);">
                            <i class="fas fa-exclamation-triangle" style="font-size: 1.5rem; margin-bottom: 10px;"></i>
                            <p>Veri yükleme hatası oluştu.</p>
                        </td>
                    </tr>`;
            }
        } catch (err) {
            console.error("Sorgu hatası:", err);
            tableBody.innerHTML = `
                <tr>
                    <td colspan="6" style="text-align: center; padding: 40px; color: var(--danger);">
                        <i class="fas fa-wifi" style="font-size: 1.5rem; margin-bottom: 10px;"></i>
                        <p>Sunucuyla bağlantı kurulamadı.</p>
                    </td>
                </tr>`;
        }
    }

    // ==========================================================
    // 2. TABLO HTML RENDER
    // ==========================================================
    function renderTable(events) {
        resultCountEl.textContent = `${events.length} sonuç listelendi`;

        if (events.length === 0) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="6" style="text-align: center; padding: 60px; color: var(--text-secondary);">
                        <i class="fas fa-inbox" style="font-size: 2.5rem; margin-bottom: 15px; color: var(--border-color);"></i>
                        <p style="font-size: 1.05rem; font-weight: 500;">Herhangi bir geçiş kaydı bulunamadı.</p>
                        <p style="font-size: 0.85rem; margin-top: 5px;">Arama kriterlerini değiştirmeyi deneyebilirsiniz.</p>
                    </td>
                </tr>`;
            return;
        }

        tableBody.innerHTML = events.map(e => {
            const categoryLower = (e.category || 'normal').toLowerCase();
            
            // Kategori Sınıfları
            let badgeClass = "normal";
            let categoryLabel = "Normal";
            if (categoryLower === "vip") { badgeClass = "vip"; categoryLabel = "VIP"; }
            else if (categoryLower === "blacklist") { badgeClass = "blacklist"; categoryLabel = "Kara Liste"; }
            else if (categoryLower === "staff") { badgeClass = "staff"; categoryLabel = "Personel"; }

            // Yön Sınıfları ve Okları
            const isEntry = e.direction === "Giriş";
            const dirClass = isEntry ? "text-success" : "text-danger";
            const dirIcon = isEntry ? "fa-arrow-right" : "fa-arrow-left";

            // Click Handler string parametrelerini düzgün kaçırmak için JSON kullanımı
            const detailParam = JSON.stringify(e).replace(/"/g, '&quot;');

            return `
                <tr style="cursor:pointer;" onclick="window.openAdvancedDetail('${e.id}')">
                    <td><span class="plate-label ${categoryLower === 'blacklist' ? 'plate-danger' : (categoryLower === 'vip' ? 'plate-warning' : 'plate-success')}">${e.plate}</span></td>
                    <td>${e.cameraName}</td>
                    <td class="${dirClass}"><i class="fas ${dirIcon}"></i> ${e.direction}</td>
                    <td class="monospace">${e.eventTimestamp}</td>
                    <td><span class="badge ${badgeClass}">${categoryLabel}</span></td>
                    <td>
                        <button class="btn-icon ${categoryLower === 'blacklist' ? 'text-danger' : 'text-info'}" title="İncele" onclick="event.stopPropagation(); window.openAdvancedDetail('${e.id}')">
                            <i class="fas fa-eye"></i>
                        </button>
                    </td>
                </tr>`;
        }).join('');
    }

    // ==========================================================
    // 3. FİLTRE YÖNETİMİ
    // ==========================================================
    window.toggleFilter = function (category, value) {
        if (category === "type") {
            if (filters.type.includes(value)) {
                filters.type = filters.type.filter(x => x !== value);
            } else {
                filters.type.push(value);
            }
        }
        if (category === "direction") {
            // Yön filtre eşleşmeleri
            filters.direction = filters.direction === value ? null : value;
            
            // Diğer yönün checkbox işaretini kaldır
            const otherVal = value === "entry" ? "exit" : "entry";
            const otherCb = document.querySelector(`.filter-content input[onclick*="toggleFilter('direction','${otherVal}')"]`) ||
                            document.querySelector(`.filter-content input[onchange*="toggleFilter('direction','${otherVal}')"]`);
            if (otherCb) otherCb.checked = false;
        }
        renderChips();
        updateResults();
    };

    window.setQuickTime = function (type) {
        filters.time = type;
        const now = new Date();
        const start = new Date();
        
        if (type === "today") {
            start.setHours(0,0,0,0);
        } else if (type === "7d") {
            start.setDate(now.getDate() - 7);
        } else if (type === "30d") {
            start.setDate(now.getDate() - 30);
        }

        // input değerlerini temizle
        document.getElementById("startDate").value = "";
        document.getElementById("endDate").value = "";

        filters.range.start = start.toISOString().split('T')[0];
        filters.range.end = now.toISOString().split('T')[0];

        renderChips();
        updateResults();
    };

    window.applyDateRange = function () {
        filters.time = null; // quick time'ı sıfırla
        const startVal = document.getElementById("startDate").value;
        const endVal = document.getElementById("endDate").value;

        // Radyo butonlarını temizle
        document.querySelectorAll("input[name='time']").forEach(rb => rb.checked = false);

        filters.range.start = startVal ? startVal : null;
        filters.range.end = endVal ? endVal : null;

        renderChips();
        updateResults();
    };

    function renderChips() {
        const box = document.getElementById("activeFilters");
        if (!box) return;
        box.innerHTML = "";

        // Tip chipleri
        filters.type.forEach(t => {
            const label = t === "vip" ? "VIP" : (t === "blacklist" ? "Kara Liste" : (t === "staff" ? "Personel" : "Normal"));
            addChip(box, `Tür: ${label}`, () => {
                const cb = document.querySelector(`.filter-content input[onchange*="toggleFilter('type','${t}')"]`);
                if (cb) cb.checked = false;
                window.toggleFilter("type", t);
            });
        });

        // Yön chipleri
        if (filters.direction) {
            const label = filters.direction === "entry" ? "Giriş" : "Çıkış";
            addChip(box, `Yön: ${label}`, () => {
                const cb = document.querySelector(`.filter-content input[onchange*="toggleFilter('direction','${filters.direction}')"]`);
                if (cb) cb.checked = false;
                window.toggleFilter("direction", filters.direction);
            });
        }

        // Zaman chipleri
        if (filters.time) {
            const label = filters.time === "today" ? "Bugün" : (filters.time === "7d" ? "Son 7 Gün" : "Son 30 Gün");
            addChip(box, `Zaman: ${label}`, () => {
                filters.time = null;
                filters.range = { start: null, end: null };
                document.querySelectorAll("input[name='time']").forEach(rb => rb.checked = false);
                renderChips();
                updateResults();
            });
        } else if (filters.range.start || filters.range.end) {
            addChip(box, "Tarih Aralığı", () => {
                filters.range = { start: null, end: null };
                document.getElementById("startDate").value = "";
                document.getElementById("endDate").value = "";
                renderChips();
                updateResults();
            });
        }
    }

    function addChip(parent, text, removeFn) {
        const chip = document.createElement("div");
        chip.className = "filter-chip";
        chip.innerHTML = `${text} <i class="fas fa-times" style="margin-left:6px; cursor:pointer; color:var(--text-secondary);"></i>`;
        chip.querySelector("i").addEventListener("click", (e) => {
            e.stopPropagation();
            removeFn();
        });
        parent.appendChild(chip);
    }

    window.resetFilters = function () {
        filters = {
            search: "",
            type: [],
            direction: null,
            time: null,
            range: { start: null, end: null }
        };

        // Form girdilerini sıfırla
        if (searchInput) searchInput.value = "";
        document.querySelectorAll(".filter-content input[type=checkbox]").forEach(cb => cb.checked = false);
        document.querySelectorAll(".filter-content input[type=radio]").forEach(rb => rb.checked = false);
        document.getElementById("startDate").value = "";
        document.getElementById("endDate").value = "";

        renderChips();
        updateResults();
    };

    // ==========================================================
    // 4. DETAY PANELİ & GEÇİŞ GEÇMİŞİ
    // ==========================================================
    window.openAdvancedDetail = function (id) {
        const e = allEvents.find(x => x.id === id);
        if (!e) return;

        detailPanel.classList.remove("hidden");

        // Basit Alanları Doldur
        document.getElementById("detailPlate").textContent = e.plate;
        document.getElementById("detailCamera").textContent = e.cameraName;
        document.getElementById("detailDirection").textContent = e.direction;
        document.getElementById("detailDate").textContent = e.eventTimestamp;

        // Kategori Rozeti Oluştur
        const categoryLower = (e.category || 'normal').toLowerCase();
        let badgeClass = "badge normal";
        let categoryLabel = "Normal";
        if (categoryLower === "vip") { badgeClass = "badge vip"; categoryLabel = "VIP"; }
        else if (categoryLower === "blacklist") { badgeClass = "badge blacklist"; categoryLabel = "Kara Liste"; }
        else if (categoryLower === "staff") { badgeClass = "badge staff"; categoryLabel = "Personel"; }
        
        const statusContainer = document.getElementById("detailStatus");
        statusContainer.innerHTML = `<span class="${badgeClass}">${categoryLabel}</span>`;

        // Görsel Yükleme (Detay paneline resim kutusu ekledik)
        const vehicleImgEl = document.getElementById("detailVehicleImg");
        if (vehicleImgEl) {
            vehicleImgEl.src = e.vehicleImg || "/img/no-car.png";
            // Tıklayınca tam boy resmi yeni sekmede açsın
            vehicleImgEl.style.cursor = "pointer";
            vehicleImgEl.onclick = () => window.open(e.fullImg || e.vehicleImg || "/img/no-car.png", "_blank");
        }

        // GEÇİŞ GEÇMİŞİ (Aynı plakalı diğer tüm olayları listeler)
        const historyList = document.querySelector(".detail-history-list");
        if (historyList) {
            historyList.innerHTML = "";
            const plateHistory = allEvents
                .filter(x => x.plate.replace(" ", "").toUpperCase() === e.plate.replace(" ", "").toUpperCase())
                // En güncel 5 geçişi göster
                .slice(0, 5);

            historyList.innerHTML = plateHistory.map(h => {
                const isEntry = h.direction === "Giriş";
                const dotColor = isEntry ? "var(--success)" : "var(--danger)";
                const dirIcon = isEntry ? "fa-arrow-right" : "fa-arrow-left";
                
                return `
                    <div class="detail-history-item" style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid var(--border-color); font-size:0.85rem;">
                        <span>
                            <i class="fas fa-circle" style="font-size:0.5rem; color:${dotColor}; margin-right:8px;"></i>
                            <strong>${h.cameraName}</strong>
                            <span style="color:var(--text-secondary); margin-left:4px;">(<i class="fas ${dirIcon}"></i>)</span>
                        </span>
                        <span class="time" style="color:var(--text-secondary); font-family:monospace;">${h.eventTimestamp.split(' ')[1]}</span>
                    </div>`;
            }).join('');
        }
    };

    window.closeDetail = function () {
        if (detailPanel) {
            detailPanel.classList.add("hidden");
        }
    };

    // Başlangıçta verileri yükle
    updateResults();
})();
