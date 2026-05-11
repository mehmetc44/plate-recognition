const liveTableStreamContainer = document.getElementById("liveTableStreamContainer");
const navLive = document.getElementById("navLiveStream");
const navPlate = document.getElementById("navPlateStream");

const liveContainer = document.getElementById("liveStreamContainer");
const plateContainer = document.getElementById("plateStreamContainer");

const btnToggleRight = document.getElementById('btnToggleRight');
const rightPanel = document.getElementById('detectionFeed');

const viewButtons = document.querySelectorAll(".view-button");

document.querySelectorAll(".nvr-toggle").forEach(node => {
    node.addEventListener("click", () => {
        const parentLi = node.closest("li");
        const nested = parentLi.querySelector(".nested");
        const caret = node.querySelector(".caret");

        const willOpen = !nested.classList.contains("open");

        nested.classList.toggle("open", willOpen);
        caret.classList.toggle("open", willOpen);
    });
});

/* Ana Ekran - Canlı İzleme ve Plaka Akışı Arası Geçiş */
// Live Stream tıklandığında
function hideViewButtons() {
    viewButtons.forEach(btn => btn.classList.add("hidden"));
}

function showViewButtons() {
    viewButtons.forEach(btn => btn.classList.remove("hidden"));
}
navLive.addEventListener("click", function () {
    liveContainer.classList.remove("hidden");
    plateContainer.classList.add("hidden");

    navLive.classList.add("active-link");
    navPlate.classList.remove("active-link");
    showViewButtons();
});
function closeTableIfOpen() {
    liveTableStreamContainer.classList.add("hidden");
}
// Plate Stream tıklandığında
navPlate.addEventListener("click", function () {

    liveContainer.classList.add("hidden");
    plateContainer.classList.remove("hidden");

    closeTableIfOpen();

    navPlate.classList.add("active-link");
    navLive.classList.remove("active-link");

    hideViewButtons();
});



/*Sağdaki Detection Feed kısmını açıp kapatan kısım */


// ilk durum: açık
let rightPanelOpen = true;

function setRightPanel(state) {
    rightPanelOpen = state;

    rightPanel.classList.toggle("hidden", !state);
    btnToggleRight.classList.toggle("active", state);
}

btnToggleRight.addEventListener("click", () => {
    setRightPanel(!rightPanelOpen);
});

// başlangıç state'i uygula
setRightPanel(true);
/*==============================================================================================*/
/*==============================================================================================*/
/* İzleme ekranını 1, 4, 12'li grid çevirme */
let currentView = 1;
function showGrid() {
    liveTableStreamContainer.classList.add("hidden");
    liveContainer.classList.remove("hidden");
}

function showTable() {
    liveContainer.classList.add("hidden");
    liveTableStreamContainer.classList.remove("hidden");
}

function renderGrid(count) {
    showGrid();

    let className = "";

    if (count === 1) className = "grid-1";
    if (count === 4) className = "grid-4";
    if (count === 12) className = "grid-12";

    liveContainer.innerHTML = "";
    liveContainer.className = `live-stream-container ${className}`;

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

/*LiveTableStream'i açıp kapatma*/
function setTableView() {
    showTable();
}
/*Butonları aktif pasif yapma*/

function setActiveButton(activeId) {
    viewButtons.forEach(btn => btn.classList.remove("active"));
    document.getElementById(activeId).classList.add("active");
}

document.getElementById("btn1View").addEventListener("click", () => {
    renderGrid(1);
    setActiveButton("btn1View");
});

document.getElementById("btn4View").addEventListener("click", () => {
    renderGrid(4);
    setActiveButton("btn4View");
});

document.getElementById("btn12View").addEventListener("click", () => {
    renderGrid(12);
    setActiveButton("btn12View");
});

document.getElementById("btnTableView").addEventListener("click", () => {
    setTableView();
    setActiveButton("btnTableView");
});

/*==============================================================================================*/
/*==============================================================================================*/
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
        disconnectHlsStream(existingStream);
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
    connectHlsStream(video, cameraId);
}