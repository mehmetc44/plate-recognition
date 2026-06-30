(function () {
    // 1. Tab toggles
    const viewModeButtons = document.querySelectorAll(".view-mode-btn");
    const viewerImages = document.querySelectorAll(".viewer-img");
    const viewModeLabel = document.getElementById("viewModeLabel");

    const viewModeLabels = {
        scene: 'Sahne Görünümü',
        vehicle: 'Araç Görünümü',
        plate: 'Plaka Görünümü'
    };

    if (viewModeButtons.length > 0) {
        viewModeButtons.forEach(button => {
            button.addEventListener("click", () => {
                const type = button.dataset.type;
                viewModeButtons.forEach(btn => btn.classList.remove("active"));
                button.classList.add("active");
                viewerImages.forEach(img => {
                    img.classList.remove("active-view");
                    if (img.dataset.view === type) {
                        img.classList.add("active-view");
                    }
                });
                if (viewModeLabel && viewModeLabels[type]) {
                    viewModeLabel.textContent = viewModeLabels[type];
                }
            });
        });
    }

    // 2. Dynamic loading
    const urlParams = new URLSearchParams(window.location.search);
    const plate = urlParams.get('plate');
    if (!plate) return;

    let vehicleData = null;
    let passesData = [];

    // DOM Elements
    const topbarPlate = document.getElementById("topbarPlate");
    const topbarLocation = document.getElementById("topbarLocation");
    const topbarBadge = document.getElementById("topbarBadge");
    const infoPlate = document.getElementById("infoPlate");
    const infoBadge = document.getElementById("infoBadge");
    const infoModel = document.getElementById("infoModel");
    const infoOwner = document.getElementById("infoOwner");
    const infoRecordDate = document.getElementById("infoRecordDate");
    const infoLastSeen = document.getElementById("infoLastSeen");
    const imgScene = document.getElementById("imgScene");
    const imgVehicle = document.getElementById("imgVehicle");
    const imgPlate = document.getElementById("imgPlate");
    const historyTableBody = document.getElementById("historyTableBody");
    const btnToggleBlacklist = document.getElementById("btnToggleBlacklist");

    function getCategoryText(cat) {
        if (!cat) return "NORMAL";
        switch (cat.toLowerCase()) {
            case "vip": return "VIP MİSAFİR";
            case "blacklist": return "KARA LİSTE";
            case "staff": return "PERSONEL";
            default: return "NORMAL";
        }
    }

    function getCategoryStyle(cat) {
        if (!cat) return { bg: "rgba(108, 117, 125, 0.15)", text: "#6c757d", icon: "fa-car" };
        switch (cat.toLowerCase()) {
            case "vip": return { bg: "rgba(255, 193, 7, 0.15)", text: "#ffc107", icon: "fa-star" };
            case "blacklist": return { bg: "rgba(220, 53, 69, 0.15)", text: "#dc3545", icon: "fa-ban" };
            case "staff": return { bg: "rgba(13, 110, 253, 0.15)", text: "#0d6efd", icon: "fa-user-tie" };
            default: return { bg: "rgba(40, 167, 69, 0.15)", text: "#28a745", icon: "fa-check-circle" };
        }
    }

    async function loadData() {
        try {
            // Load vehicle registration
            const vRes = await fetch(`/Vehicle/GetVehicleByPlateApi?plate=${encodeURIComponent(plate)}`);
            const vData = await vRes.json();
            if (vData.success) {
                vehicleData = vData;
                bindVehicleInfo();
            }

            // Load transition history
            const hRes = await fetch(`/api/lpr/history?plate=${encodeURIComponent(plate)}`);
            passesData = await hRes.json();
            bindHistory();
        } catch (err) {
            console.error("Detay yükleme hatası:", err);
        }
    }

    function bindVehicleInfo() {
        if (!vehicleData) return;

        topbarPlate.textContent = vehicleData.plate;
        infoPlate.textContent = vehicleData.plate;

        infoModel.textContent = vehicleData.model;
        infoOwner.textContent = vehicleData.owner;
        infoRecordDate.textContent = vehicleData.date;

        const catText = getCategoryText(vehicleData.category);
        const catStyle = getCategoryStyle(vehicleData.category);

        const badgeHtml = `<i class="fas ${catStyle.icon}"></i> ${catText}`;
        topbarBadge.innerHTML = badgeHtml;
        topbarBadge.style.background = catStyle.bg;
        topbarBadge.style.color = catStyle.text;
        topbarBadge.style.border = `1px solid ${catStyle.text}`;

        infoBadge.innerHTML = badgeHtml;
        infoBadge.style.background = catStyle.bg;
        infoBadge.style.color = catStyle.text;
        infoBadge.style.border = `1px solid ${catStyle.text}`;

        // Blacklist Button status
        if (vehicleData.category === "blacklist") {
            btnToggleBlacklist.innerHTML = `<i class="fas fa-check-circle"></i> Kara Listeden Çıkar`;
            btnToggleBlacklist.className = "btn-secondary-action";
            btnToggleBlacklist.style.color = "var(--text-primary)";
            btnToggleBlacklist.style.background = "rgba(255, 255, 255, 0.1)";
        } else {
            btnToggleBlacklist.innerHTML = `<i class="fas fa-ban"></i> Kara Listeye Ekle`;
            btnToggleBlacklist.className = "btn-danger-action";
            btnToggleBlacklist.style.color = "";
            btnToggleBlacklist.style.background = "";
        }
    }

    function bindImages(pass) {
        if (!pass) {
            imgScene.src = "/img/no-car.png";
            imgVehicle.src = "/img/no-car.png";
            imgPlate.src = "/img/no-car.png";
            return;
        }

        imgScene.src = pass.fullImg || "/img/no-car.png";
        imgVehicle.src = pass.vehicleImg || "/img/no-car.png";
        imgPlate.src = pass.plateImg || "/img/no-car.png";
    }

    function bindHistory() {
        if (!passesData || passesData.length === 0) {
            historyTableBody.innerHTML = `<tr><td colspan="5" style="text-align: center; color: var(--text-secondary);">Bu plaka için geçiş geçmişi bulunamadı.</td></tr>`;
            topbarLocation.innerHTML = `<i class="fas fa-map-marker-alt"></i> Bilinmiyor`;
            infoLastSeen.textContent = "Görülmedi";
            bindImages(null);
            return;
        }

        // Set topbar location to last seen location
        const lastPass = passesData[0];
        topbarLocation.innerHTML = `<i class="fas fa-map-marker-alt"></i> ${lastPass.cameraName} (${lastPass.direction})`;
        infoLastSeen.textContent = `${lastPass.cameraName} - ${lastPass.eventTimestamp}`;

        // Set initial photos to last pass
        bindImages(lastPass);

        // Populate table
        const catStyle = getCategoryStyle(vehicleData ? vehicleData.category : "normal");
        const statusBadge = `<span class="badge-status" style="background: ${catStyle.bg}; color: ${catStyle.text}; border: 1px solid ${catStyle.text}; font-size: 0.75rem; padding: 2px 6px; border-radius: 4px;">${getCategoryText(vehicleData ? vehicleData.category : "normal")}</span>`;

        historyTableBody.innerHTML = passesData.map((pass, index) => {
            const isDirGiris = pass.direction === "Giriş";
            const dirColor = isDirGiris ? "text-success" : "text-danger";
            const dirIcon = isDirGiris ? "fa-arrow-right" : "fa-arrow-left";

            return `
                <tr style="cursor: pointer;" class="history-row" data-index="${index}">
                    <td class="monospace">${pass.eventTimestamp}</td>
                    <td>${pass.cameraName}</td>
                    <td><span class="${dirColor}"><i class="fas ${dirIcon}"></i> ${pass.direction}</span></td>
                    <td>${statusBadge}</td>
                    <td>
                        <button class="btn-icon-action text-info" title="Fotoğrafları Göster" onclick="event.stopPropagation(); window.showPassMedia(${index})">
                            <i class="fas fa-image"></i>
                        </button>
                    </td>
                </tr>
            `;
        }).join('');

        // Add row clicks
        document.querySelectorAll(".history-row").forEach(row => {
            row.addEventListener("click", () => {
                const idx = parseInt(row.dataset.index);
                window.showPassMedia(idx);
            });
        });
    }

    // Helper exposed to global window scope so onclick handles inside HTML work
    window.showPassMedia = function (index) {
        if (passesData && passesData[index]) {
            bindImages(passesData[index]);
            // Highlight selected row
            const rows = document.querySelectorAll(".history-row");
            rows.forEach(r => r.style.background = "");
            const selectedRow = document.querySelector(`.history-row[data-index="${index}"]`);
            if (selectedRow) {
                selectedRow.style.background = "rgba(255, 255, 255, 0.05)";
            }
        }
    };

    // Blacklist button toggle click event
    if (btnToggleBlacklist) {
        btnToggleBlacklist.addEventListener("click", async () => {
            if (!vehicleData) return;

            const isCurrentlyBlacklist = vehicleData.category === "blacklist";
            const newCategory = isCurrentlyBlacklist ? "normal" : "blacklist";

            // If vehicle doesn't exist, we add it. If it does, we update it.
            const url = vehicleData.exists ? "/Vehicle/UpdateVehicleApi" : "/Vehicle/AddVehicleApi";
            const payload = {
                plate: vehicleData.plate,
                model: vehicleData.model,
                owner: vehicleData.owner,
                category: newCategory,
                note: isCurrentlyBlacklist ? "Kara listeden çıkarıldı." : "Araç detayı sayfasından kara listeye eklendi."
            };

            if (vehicleData.exists) {
                payload.id = vehicleData.id;
            }

            try {
                btnToggleBlacklist.disabled = true;
                const res = await fetch(url, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(payload)
                });
                const resData = await res.json();
                if (resData.success) {
                    // Refresh data
                    await loadData();
                    if (window.opener && typeof window.opener.loadVehicles === "function") {
                        // Refresh parent page lists if it's open
                        window.opener.loadVehicles();
                    }
                } else {
                    alert(resData.message || "İşlem başarısız oldu.");
                }
            } catch (err) {
                console.error("Kara liste değiştirme hatası:", err);
            } finally {
                btnToggleBlacklist.disabled = false;
            }
        });
    }

    // Init load
    loadData();
})();