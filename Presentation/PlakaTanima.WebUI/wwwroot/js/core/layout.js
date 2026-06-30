// ==========================================
// GLOBAL FETCH INTERCEPTOR FOR JWT REFRESH
// ==========================================
(function () {
    const originalFetch = window.fetch;
    let isRefreshing = false;
    let refreshSubscribers = [];

    function subscribeTokenRefresh(cb) {
        refreshSubscribers.push(cb);
    }

    function onTokenRefreshed() {
        refreshSubscribers.forEach(cb => cb());
        refreshSubscribers = [];
    }

    window.fetch = async function (...args) {
        let response = await originalFetch(...args);

        // Intercept 401 Unauthorized
        if (response.status === 401) {
            const url = args[0];
            // Avoid loop if refresh token endpoint fails
            if (typeof url === 'string' && url.includes('/api/auth/refresh')) {
                return response;
            }

            if (!isRefreshing) {
                isRefreshing = true;
                try {
                    const refreshRes = await originalFetch('/api/auth/refresh', {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' }
                    });

                    if (refreshRes.ok) {
                        isRefreshing = false;
                        onTokenRefreshed();
                    } else {
                        isRefreshing = false;
                        localStorage.removeItem('platar_user');
                        window.location.href = '/login';
                        return response;
                    }
                } catch (err) {
                    isRefreshing = false;
                    localStorage.removeItem('platar_user');
                    window.location.href = '/login';
                    return response;
                }
            }

            return new Promise((resolve) => {
                subscribeTokenRefresh(async () => {
                    resolve(await originalFetch(...args));
                });
            });
        }

        return response;
    };
})();

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
      logoutBtn.addEventListener('click', async function (e) {
        e.preventDefault();
        try {
          // Sunucudaki oturumu kapat (refresh token'ı iptal et ve çerezleri temizle)
          await fetch('/api/auth/logout', { method: 'POST' });
        } catch (err) { /* ignore */ }
        
        // localStorage'dan kullanıcı bilgilerini temizle
        try {
          localStorage.removeItem('platar_user');
        } catch (err) { /* ignore */ }
        
        // Login sayfasına yönlendir
        window.location.href = '/login';
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