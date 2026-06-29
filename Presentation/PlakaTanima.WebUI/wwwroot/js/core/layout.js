document.addEventListener("turbo:load", () => {
  // ==========================================================
  // 1. AKTİF NAVBAR LİNK'İNİ BELİRLE
  // ==========================================================
  const path = window.location.pathname.split("/").pop();
  const navLinks = document.querySelectorAll(".nav-links a");

  navLinks.forEach(link => {
    const href = link.getAttribute("href");
    if (href === path) {
      link.classList.add("active");
    } else {
      link.classList.remove("active");
    }
    link.addEventListener("click", () => {
      navLinks.forEach(l => l.classList.remove("active"));
      link.classList.add("active");
    });
  });

  // ==========================================================
  // 2. USER PROFILE DROPDOWN
  // ==========================================================
  const userProfile = document.querySelector(".user-profile");
  if (userProfile) {
    userProfile.addEventListener("click", function (e) {
      e.stopPropagation();
      this.classList.toggle("open");
    });

    // Dropdown dışına tıklayınca kapat
    document.addEventListener("click", function () {
      userProfile.classList.remove("open");
    });

    // Dropdown içine tıklayınca kapanmasın
    const dropdown = userProfile.querySelector(".user-dropdown");
    if (dropdown) {
      dropdown.addEventListener("click", function (e) {
        e.stopPropagation();
      });
    }

    // Çıkış Yap butonuna özel işlev
    const logoutBtn = userProfile.querySelector('.dropdown-item.danger');
    if (logoutBtn) {
      logoutBtn.addEventListener('click', function (e) {
        e.preventDefault();
        // localStorage'dan kullanıcı bilgilerini temizle
        try {
          localStorage.removeItem('platar_user');
        } catch (err) { /* ignore */ }
        // Login sayfasına yönlendir
        window.location.href = 'login.html';
      });
    }
  }

  // ==========================================================
  // 3. PLAKA ARAMA / FİLTRE (index.html için)
  // ==========================================================
  const searchInput = document.getElementById("plateSearch");
  const filterButtons = document.querySelectorAll(".filter-btn");
  const rows = document.querySelectorAll(".vehicle-table tbody tr");

  if (searchInput && rows.length) {
    let activeFilter = "all";
    let searchValue = "";

    function applyFilters() {
      rows.forEach(row => {
        const plate = row.querySelector(".plate-label")?.textContent.toLowerCase() || "";
        const type = row.dataset.type;
        const matchFilter = activeFilter === "all" || type === activeFilter;
        const matchSearch = plate.includes(searchValue);
        row.style.display = (matchFilter && matchSearch) ? "" : "none";
      });
    }

    filterButtons.forEach(btn => {
      btn.addEventListener("click", () => {
        filterButtons.forEach(b => b.classList.remove("active"));
        btn.classList.add("active");
        activeFilter = btn.dataset.filter;
        applyFilters();
      });
    });

    searchInput.addEventListener("input", (e) => {
      searchValue = e.target.value.toLowerCase();
      applyFilters();
    });
  }
});