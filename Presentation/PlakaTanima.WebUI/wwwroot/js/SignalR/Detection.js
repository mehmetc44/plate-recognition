"use strict";

// Multi-connection and handler duplication prevention (fixes duplicates caused by page transitions/Turbo reload)
if (!window.signalrConnection) {
    window.signalrConnection = new signalR.HubConnectionBuilder()
        .withUrl("/plateHub")
        .withAutomaticReconnect()
        .build();

    window.signalrConnection.start().then(function () {
        console.log("SignalR bağlantısı başarılı. Canlı plaka akışı dinleniyor...");
    }).catch(err => console.error("SignalR başlatılamadı: " + err.toString()));
}

// Clear any previous listeners on the connection to prevent duplicate handling
window.signalrConnection.off("NewPlateDetected");

window.signalrConnection.on("NewPlateDetected", function (data) {
    const feed = document.getElementById("detectionFeed");
    if (!feed) return;

    // 1. Resolve image source (supports base64, MinIO URLs, and fallback)
    let vehicleImgSrc = "/img/no-car.png";
    if (data.imgVehicle) {
        if (data.imgVehicle.startsWith("http") || data.imgVehicle.startsWith("/")) {
            vehicleImgSrc = data.imgVehicle;
        } else {
            vehicleImgSrc = `data:image/jpeg;base64,${data.imgVehicle}`;
        }
    }

    // 2. Resolve category specific badge classes, icons, labels and card accent borders
    let badgeClass = "badge-normal";
    let badgeIcon = "fa-check-circle";
    let badgeText = "Normal";
    let cardAccentClass = "";

    const categoryLower = (data.category || "normal").toLowerCase();

    if (categoryLower === "vip") {
        badgeClass = "badge-vip";
        badgeIcon = "fa-star";
        badgeText = "VIP";
        cardAccentClass = "status-warning";
    } else if (categoryLower === "blacklist" || categoryLower === "kara liste") {
        badgeClass = "badge-alert";
        badgeIcon = "fa-ban";
        badgeText = "KARA LİSTE";
        cardAccentClass = "status-danger";
    } else if (categoryLower === "staff" || categoryLower === "personel") {
        badgeClass = "badge-staff";
        badgeIcon = "fa-id-badge";
        badgeText = "PERSONEL";
    }

    // 3. Generate and slide in the vehicle card HTML
    const cardHtml = `
        <div class="vehicle-card ${cardAccentClass} animate__animated animate__fadeInDown">
            <!-- Dynamic Status Badge -->
            <span class="badge ${badgeClass} badge-absolute">
                <i class="fas ${badgeIcon}"></i> ${badgeText}
            </span>

            <!-- Vehicle Snapshot Image -->
            <img src="${vehicleImgSrc}" alt="Araç" class="vehicle-img">

            <div class="vehicle-info">
                <!-- Plate Monospace Label -->
                <div class="plate-label ${categoryLower === 'blacklist' ? 'plate-danger' : (categoryLower === 'vip' ? 'plate-warning' : 'plate-success')}">${data.plate}</div>
                
                <!-- Camera Location -->
                <div class="text-success small mt-1">
                    <i class="fas fa-arrow-right"></i> ${data.camera}
                </div>
                
                <!-- Detection DateTime -->
                <div class="text-muted small">
                    <i class="far fa-clock"></i> ${data.fullTime}
                </div>
            </div>
        </div>`;

    feed.insertAdjacentHTML('afterbegin', cardHtml);

    // Limit active live feed cards to 20 to prevent DOM overflow
    if (feed.children.length > 20) {
        feed.lastElementChild.remove();
    }

    // Attach click event dynamically to the newly added card (for detail redirection)
    const newCard = feed.firstElementChild;
    if (newCard) {
        newCard.addEventListener("click", function () {
            const plateEl = this.querySelector(".plate-label");
            if (plateEl) {
                const plate = plateEl.textContent.trim();
                window.open(`/Home/VehicleDetails?plate=${encodeURIComponent(plate)}`, "_blank");
            }
        });
    }
});