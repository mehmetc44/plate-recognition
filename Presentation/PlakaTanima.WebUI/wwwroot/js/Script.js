
document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll(".nav-links a");

    function setActive(element) {
        links.forEach(l => l.classList.remove("active"));
        element.classList.add("active");
        localStorage.setItem("activeNav", element.id);
    }

    // 1) Daha önce seçim varsa onu yükle
    const saved = localStorage.getItem("activeNav");

    if (saved) {
        const savedEl = document.getElementById(saved);
        if (savedEl) {
            setActive(savedEl);
        }
    } 
    // 2) Yoksa ilk elemanı seç
    else if (links.length > 0) {
        setActive(links[0]);
    }

    // click event
    links.forEach(link => {
        link.addEventListener("click", function (e) {
            setActive(this);
        });
    });
});
