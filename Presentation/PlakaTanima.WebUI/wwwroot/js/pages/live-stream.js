(function () {
    const liveContainer = document.getElementById("liveStreamContainer");
    if (!liveContainer) return; // Page guard: Canlı akış sayfasında değilsek çalıştırma!

    const liveTableStreamContainer = document.getElementById("liveTableStreamContainer");
    const navLive = document.getElementById("navLiveStream");
    const navPlate = document.getElementById("navPlateStream");
    const navTable = document.getElementById("navTableStream");
    const plateContainer = document.getElementById("plateStreamContainer");
    const btnToggleRight = document.getElementById('btnToggleRight');
    const rightPanel = document.getElementById('detectionFeed');
    const viewButtons = document.querySelectorAll(".view-button");



    /* Ana Ekran - Canlı İzleme ve Plaka Akışı Arası Geçiş */
    function hideViewButtons() {
        viewButtons.forEach(btn => btn.classList.add("hidden"));
    }

    function showViewButtons() {
        viewButtons.forEach(btn => btn.classList.remove("hidden"));
    }

    if (navLive) {
        navLive.addEventListener("click", function () {
            liveContainer.classList.remove("hidden");
            liveContainer.style.display = "";
            plateContainer.classList.add("hidden");
            liveTableStreamContainer.classList.add("hidden");

            navLive.classList.add("active-link");
            navPlate.classList.remove("active-link");
            navTable.classList.remove("active-link");
            showViewButtons();

            // Eğer hiç grid render edilmemişse varsayılan 1'li gridi göster
            if (liveContainer.children.length === 0) {
                renderGrid(1);
                setActiveButton("btn1View");
            }
        });
    }

    function closeTableIfOpen() {
        if (liveTableStreamContainer) {
            liveTableStreamContainer.classList.add("hidden");
        }
    }

    // Plate Stream tıklandığında
    if (navPlate) {
        navPlate.addEventListener("click", function () {
            liveContainer.classList.add("hidden");
            plateContainer.classList.remove("hidden");

            closeTableIfOpen();

            navPlate.classList.add("active-link");
            navLive.classList.remove("active-link");
            navTable.classList.remove("active-link");

            hideViewButtons();
        });
    }

    // Liste Akışı tıklandığında
    if (navTable) {
        navTable.addEventListener("click", function () {
            liveContainer.classList.add("hidden");
            plateContainer.classList.add("hidden");

            liveTableStreamContainer.classList.remove("hidden");

            navTable.classList.add("active-link");
            navLive.classList.remove("active-link");
            navPlate.classList.remove("active-link");

            hideViewButtons();
        });
    }

    /*Sağdaki Detection Feed kısmını açıp kapatan kısım */
    let rightPanelOpen = true;

    function setRightPanel(state) {
        rightPanelOpen = state;
        if (rightPanel) rightPanel.classList.toggle("hidden", !state);
        if (btnToggleRight) btnToggleRight.classList.toggle("active", state);
    }

    if (btnToggleRight) {
        btnToggleRight.addEventListener("click", () => {
            setRightPanel(!rightPanelOpen);
        });
    }

    // başlangıç state'i uygula
    setRightPanel(true);

    /*==============================================================================================*/
    /* İzleme ekranını 1, 4, 12'li grid çevirme */
    function showGrid() {
        liveContainer.style.display = "";
        if (liveTableStreamContainer) liveTableStreamContainer.classList.add("hidden");
        if (plateContainer) plateContainer.classList.add("hidden");
        liveContainer.classList.remove("hidden");
    }

    function renderGrid(count) {
        showGrid();

        // Container class'larını temizle ve yeni grid class'ını ekle
        liveContainer.classList.remove("grid-1", "grid-4", "grid-12");
        liveContainer.classList.add(`grid-${count}`);

        liveContainer.innerHTML = "";

        for (let i = 1; i <= count; i++) {
            const slot = document.createElement("div");
            slot.className = "video-slot drop-zone";
            slot.dataset.slotId = i;

            slot.innerHTML = `
                <div class="camera-overlay-info">
                    <div>Kamera ${i}</div>
                    <div>Boş Slot</div>
                </div>
                <div class="no-signal">Kamera Seçin</div>
            `;

            // 🔥 DROP ENABLE
            slot.addEventListener("dragover", (e) => {
                e.preventDefault();
            });

            slot.addEventListener("drop", (e) => {
                e.preventDefault();
                const cameraId = e.dataTransfer.getData("cameraId");
                attachCameraToSlot(slot, cameraId);
            });

            liveContainer.appendChild(slot);
        }
    }

    /*Butonları aktif pasif yapma*/
    function setActiveButton(activeId) {
        viewButtons.forEach(btn => btn.classList.remove("active"));
        const btn = document.getElementById(activeId);
        if (btn) btn.classList.add("active");
    }

    const btn1View = document.getElementById("btn1View");
    if (btn1View) {
        btn1View.addEventListener("click", () => {
            renderGrid(1);
            setActiveButton("btn1View");
        });
    }

    const btn4View = document.getElementById("btn4View");
    if (btn4View) {
        btn4View.addEventListener("click", () => {
            renderGrid(4);
            setActiveButton("btn4View");
        });
    }

    const btn12View = document.getElementById("btn12View");
    if (btn12View) {
        btn12View.addEventListener("click", () => {
            renderGrid(12);
            setActiveButton("btn12View");
        });
    }

    /* SÜRÜKLE BIRAK KISMI - DRAG & DROP */
    /* Kamera öğelerini sürüklenebilir yapma */
    document.querySelectorAll(".camera-item").forEach(cam => {
        cam.setAttribute("draggable", true);
        cam.addEventListener("dragstart", (e) => {
            e.dataTransfer.setData("cameraId", cam.dataset.cameraId);
        });
    });

    /* DROP ZONE'ları aktif etme (SÜRÜKLEME EFEKTİ)*/
    document.querySelectorAll(".drop-zone").forEach(slot => {
        slot.addEventListener("dragenter", () => {
            slot.classList.add("dragover");
        });

        slot.addEventListener("dragleave", () => {
            slot.classList.remove("dragover");
        });

        slot.addEventListener("drop", () => {
            slot.classList.remove("dragover");
        });
    });

    /* Bir kamerayı slot'a yerleştirme fonksiyonu (Görüntü getirme fonksiyonu)*/
    function attachCameraToSlot(slot, cameraId) {
        const existingStream = slot.dataset.stream;

        // 🔥 1. Aynı kamera tekrar bağlanmasın
        if (existingStream === String(cameraId)) {
            console.log("Aynı kamera zaten bağlı:", cameraId);
            return;
        }

        // 🔥 2. Eski stream'i temizle
        if (existingStream) {
            if (typeof disconnectHlsStream === "function") {
                disconnectHlsStream(existingStream);
            }
        }

        // slot state update
        slot.dataset.stream = cameraId;

        // UI reset
        slot.innerHTML = "";

        // overlay
        const overlay = document.createElement("div");
        overlay.className = "camera-overlay-info";
        overlay.innerHTML = `
            <div>Kamera ${cameraId}</div>
            <div>CANLI AKTİF</div>
        `;
        slot.appendChild(overlay);

        // video
        const video = document.createElement("video");
        video.autoplay = true;
        video.muted = true;
        video.playsInline = true;
        video.className = "live-video";

        slot.appendChild(video);

        // 🔥 STREAM BAĞLAMA (ENGINE)
        if (typeof connectHlsStream === "function") {
            connectHlsStream(video, cameraId);
        }
    }

    /* TESPİT KARTLARINA TIKLAYINCA arac_detay.html'A YÖNLENDİR */
    document.querySelectorAll(".vehicle-card").forEach(card => {
        card.addEventListener("click", function () {
            // Karttaki plakayı bul
            const plateEl = this.querySelector(".plate-label");
            if (plateEl) {
                const plate = plateEl.textContent.trim();
                window.open(`/Home/VehicleDetails?plate=${encodeURIComponent(plate)}`, "_blank");
            }
        });
    });

    /* LİSTE AKIŞI TABLOSUNDAKİ İNCELE BUTONLARI (EVENT DELEGATION) */
    const tableStreamBody = document.getElementById("liveTableStreamBody");
    if (tableStreamBody) {
        tableStreamBody.addEventListener("click", function (e) {
            const btn = e.target.closest(".btn-table-action");
            if (btn) {
                e.stopPropagation();
                const plate = btn.dataset.plate;
                if (plate) {
                    window.open(`/Home/VehicleDetails?plate=${encodeURIComponent(plate)}`, "_blank");
                }
            }
        });
    }

    // Canlı Tablo Arama ve Filtreleme
    const plateSearch = document.getElementById("plateSearch");
    const tableFilterButtons = document.querySelectorAll(".filter-btn");

    let currentFilter = "all";
    let currentSearch = "";

    function filterTableRows() {
        if (!tableStreamBody) return;
        const rows = tableStreamBody.querySelectorAll("tr");
        rows.forEach(row => {
            const plate = row.querySelector(".plate-label")?.textContent.toLowerCase() || "";
            const type = row.dataset.type || "normal";
            const matchFilter = currentFilter === "all" || type.toLowerCase() === currentFilter.toLowerCase();
            const matchSearch = plate.includes(currentSearch);
            row.style.display = (matchFilter && matchSearch) ? "" : "none";
        });
    }

    if (plateSearch) {
        plateSearch.addEventListener("input", (e) => {
            currentSearch = e.target.value.toLowerCase();
            filterTableRows();
        });
    }

    if (tableFilterButtons.length > 0) {
        tableFilterButtons.forEach(btn => {
            btn.addEventListener("click", () => {
                tableFilterButtons.forEach(b => b.classList.remove("active"));
                btn.classList.add("active");
                currentFilter = btn.dataset.filter;
                filterTableRows();
            });
        });
    }

    // Expose helpers globally so Detection.js can read them when adding new rows
    window.getCurrentTableFilters = function () {
        return {
            filter: currentFilter,
            search: currentSearch
        };
    };

    // Kamera Arama Filtresi
    const cameraSearch = document.getElementById("cameraSearch");
    if (cameraSearch) {
        cameraSearch.addEventListener("input", (e) => {
            const text = e.target.value.toLowerCase();
            
            // 1. Kameraları filtrele
            document.querySelectorAll("#homeCameraTree .camera-item").forEach(item => {
                const name = item.textContent.toLowerCase();
                const ip = item.dataset.ip || "";
                const isMatch = name.includes(text) || ip.includes(text);
                item.style.display = isMatch ? "" : "none";
            });

            // 2. Klasörleri (details) filtrele ve arama yapılıyorsa otomatik aç
            document.querySelectorAll("#homeCameraTree > li").forEach(node => {
                const details = node.querySelector("details");
                if (!details) return;

                const total = details.querySelectorAll(".camera-item").length;
                const hidden = details.querySelectorAll(".camera-item[style*='display: none']").length;

                if (total > 0 && total === hidden) {
                    node.style.display = "none";
                } else {
                    node.style.display = "";
                    if (text.length > 0 && total > hidden) {
                        details.open = true;
                    }
                }
            });
        });
    }

    // BAŞLANGIÇ: Canlı izleme ekranını 1'li grid olarak yükle
    renderGrid(1);
    setActiveButton("btn1View");
})();