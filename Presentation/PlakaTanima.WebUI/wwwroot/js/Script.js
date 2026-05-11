document.addEventListener("DOMContentLoaded", () => {
  const btn = document.getElementById("themeToggle");
  const html = document.documentElement;

  // 🔥 Sayfa açılınca tema yükle
  const savedTheme = localStorage.getItem("theme");

  if (savedTheme === "dark") {
    html.dataset.theme = "dark";
  }

  btn.addEventListener("click", () => {
    const isDark = html.dataset.theme === "dark";

    if (isDark) {
      delete html.dataset.theme;
      localStorage.setItem("theme", "light");
    } else {
      html.dataset.theme = "dark";
      localStorage.setItem("theme", "dark");
    }
  });
});

document.addEventListener("DOMContentLoaded", () => {
  const searchInput = document.getElementById("plateSearch");
  const filterButtons = document.querySelectorAll(".filter-btn");
  const rows = document.querySelectorAll(".vehicle-table tbody tr");

  let activeFilter = "all";
  let searchValue = "";

  // 🔥 FILTER FUNCTION
  function applyFilters() {
    rows.forEach(row => {
      const plate = row.querySelector(".plate-label")?.textContent.toLowerCase() || "";
      const type = row.dataset.type;

      const matchFilter =
        activeFilter === "all" || type === activeFilter;

      const matchSearch =
        plate.includes(searchValue);

      if (matchFilter && matchSearch) {
        row.style.display = "";
      } else {
        row.style.display = "none";
      }
    });
  }

  // 🔥 BUTTON FILTERS
  filterButtons.forEach(btn => {
    btn.addEventListener("click", () => {
      filterButtons.forEach(b => b.classList.remove("active"));
      btn.classList.add("active");

      activeFilter = btn.dataset.filter;
      applyFilters();
    });
  });

  // 🔥 SEARCH FILTER
  searchInput.addEventListener("input", (e) => {
    searchValue = e.target.value.toLowerCase();
    applyFilters();
  });





  document.addEventListener("DOMContentLoaded", () => {
  const path = window.location.pathname.toLowerCase();

  // Önce hepsini temizle
  const links = [
    document.getElementById("navLive"),
    document.getElementById("navQuery"),
    document.getElementById("navDatabase"),
    document.getElementById("navSettings")
  ];

  links.forEach(l => l?.classList.remove("active"));

  // Route bazlı aktif yap
  if (path.includes("home") || path === "/" ) {
    document.getElementById("navLive")?.classList.add("active");
  }

  if (path.includes("gelismis") || path.includes("query")) {
    document.getElementById("navQuery")?.classList.add("active");
  }

  if (path.includes("watchlist") || path.includes("database")) {
    document.getElementById("navDatabase")?.classList.add("active");
  }

  if (path.includes("settings")) {
    document.getElementById("navSettings")?.classList.add("active");
  }
});
});