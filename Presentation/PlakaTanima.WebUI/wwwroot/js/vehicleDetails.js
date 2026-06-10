
const viewModeButtons = document.querySelectorAll(".view-mode-btn");
const viewerImages = document.querySelectorAll(".viewer-img");
const viewModeLabel = document.getElementById("viewModeLabel");

const viewModeLabels = {
    scene: 'Sahne Görünümü',
    vehicle: 'Araç Görünümü',
    plate: 'Plaka Görünümü'
};

viewModeButtons.forEach(button => {
    button.addEventListener("click", () => {
        const type = button.dataset.type;

        // Buton aktifliğini güncelle
        viewModeButtons.forEach(btn => btn.classList.remove("active"));
        button.classList.add("active");

        // Görsel değişimi
        viewerImages.forEach(img => {
            img.classList.remove("active-view");
            if (img.dataset.view === type) {
                img.classList.add("active-view");
            }
        });

        // Başlık güncelle
        if (viewModeLabel && viewModeLabels[type]) {
            viewModeLabel.textContent = viewModeLabels[type];
        }
    });
});