"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/plateHub")
    .withAutomaticReconnect()
    .build();
connection.on("NewPlateDetected", function (data) {
    const feed = document.getElementById("detectionFeed");

    const vehicleImgSrc = data.imgVehicle 
        ? `data:image/jpeg;base64,${data.imgVehicle}` 
        : "/img/no-car.png";

    // Senin tasarımın - Tam isabet verilerle
    const cardHtml = `
        <div class="vehicle-card animate__animated animate__fadeInDown">
            <!-- Statü: Şimdilik hepsi Normal -->
            <span class="badge badge-normal badge-absolute">
                <i class="fas fa-check-circle"></i> Normal
            </span>

            <!-- Araç Görseli -->
            <img src="${vehicleImgSrc}" alt="Araç" class="vehicle-img">

            <div class="vehicle-info">
                <!-- Plaka (38UC997 gibi) -->
                <div class="plate-label plate-success">${data.plate}</div>
                
                <!-- Alt Bilgi: Kamera İsmi ve Tam Tarih/Saat -->
                <div class="text-success small mt-1">
                    <i class="fas fa-arrow-right"></i> ${data.camera}
                </div>
                <div class="text-muted small">
                    <i class="far fa-clock"></i> ${data.fullTime}
                </div>
            </div>
        </div>`;

    feed.insertAdjacentHTML('afterbegin', cardHtml);

    if (feed.children.length > 20) {
        feed.lastElementChild.remove();
    }
});

connection.start().then(function () {
    console.log("Bağlantı başarılı. Akış dinleniyor...");
}).catch(err => console.error(err.toString()));