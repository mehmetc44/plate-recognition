/**
 * ============================================================
 *  PLATAR — GLOBAL TEMA YÖNETİM MOTORU (v2.0)
 *  Tüm sayfalarda tutarlı tema deneyimi sağlar.
 *  ============================================================
 *  Kullanım:
 *    1. Tüm HTML sayfalarında <head> içinde çağırın:
 *       <script src="js/theme.js"></script>
 *    2. Ayarlar sayfasında seçilen tema, vurgu rengi ve
 *       yoğunluk otomatik olarak tüm sekmelere yansır.
 *    3. Farklı sekmede değişiklik yapılırsa storage event'i
 *       ile anlık senkronizasyon sağlanır.
 * ============================================================
 */

(function () {
  "use strict";

  const html = document.documentElement;

  // ==========================================================
  // 1. TEMA SABİTLERİ
  // ==========================================================
  const THEMES = ["light", "ocean", "forest", "royal", "sunset", "slate"];
  const ACCENT_COLORS = ["blue", "red", "green", "orange", "purple", "cyan", "pink"];
  const DENSITIES = ["compact", "normal", "comfortable"];

  // ==========================================================
  // 2. LOCALSTORAGE ANAHTARLARI (merkezi yönetim)
  // ==========================================================
  const KEYS = {
    THEME_BASE: "theme-base",
    THEME_DARK: "theme-dark",
    ACCENT_COLOR: "accent-color",
    TABLE_DENSITY: "table-density",
  };

  // ==========================================================
  // 3. TEMA OKUMA YARDIMCILARI
  // ==========================================================
  function getStoredValue(key, defaultValue) {
    try {
      return localStorage.getItem(key) || defaultValue;
    } catch {
      return defaultValue;
    }
  }

  /**
   * Kaydedilmiş tema yapılandırmasını döndürür.
   * @returns {{ base: string, dark: boolean }}
   */
  function getTheme() {
    return {
      base: getStoredValue(KEYS.THEME_BASE, "light"),
      dark: getStoredValue(KEYS.THEME_DARK) === "1",
    };
  }

  /**
   * Kaydedilmiş vurgu rengini döndürür.
   * @returns {string}
   */
  function getAccentColor() {
    return getStoredValue(KEYS.ACCENT_COLOR, "blue");
  }

  /**
   * Kaydedilmiş tablo yoğunluğunu döndürür.
   * @returns {string}
   */
  function getTableDensity() {
    return getStoredValue(KEYS.TABLE_DENSITY, "normal");
  }

  // ==========================================================
  // 4. TEMA UYGULAMA
  // ==========================================================

  /**
   * Tema, vurgu rengi ve yoğunluğu HTML data attribute'larına
   * uygular. Tüm CSS değişkenleri bu attribute'lar üzerinden
   * çalışır.
   *
   * @param {string} base          - Tema adı (light, ocean, forest, ...)
   * @param {boolean} [dark=false] - Koyu varyant aktif mi?
   */
  function applyTheme(base, dark) {
    // --- Tema hesaplama ---
    let theme;
    if (base === "light") {
      theme = dark ? "dark" : "light"; // "light" açıkça set edilir
    } else {
      theme = dark ? `${base}-dark` : base;
    }

    // --- HTML data-theme attribute ---
    html.dataset.theme = theme;

    // --- localStorage'a yaz ---
    try {
      localStorage.setItem(KEYS.THEME_BASE, base);
      localStorage.setItem(KEYS.THEME_DARK, dark ? "1" : "0");
    } catch {
      // localStorage dolu olabilir, sessizce geç
    }
  }

  /**
   * Vurgu rengini HTML'e uygular.
   * @param {string} color - Vurgu rengi adı (blue, red, green, ...)
   */
  function applyAccentColor(color) {
    html.dataset.accent = color;
    try {
      localStorage.setItem(KEYS.ACCENT_COLOR, color);
    } catch {
      // sessizce geç
    }
  }

  /**
   * Tablo yoğunluğunu HTML'e uygular.
   * @param {string} density - Yoğunluk (compact, normal, comfortable)
   */
  function applyTableDensity(density) {
    html.dataset.density = density;
    try {
      localStorage.setItem(KEYS.TABLE_DENSITY, density);
    } catch {
      // sessizce geç
    }
  }

  // ==========================================================
  // 5. İLK ÇALIŞTIRMA (init)
  // ==========================================================

  /**
   * Sayfa yüklendiğinde kaydedilmiş temayı yükler ve uygular.
   * Bu fonksiyon hem DOMContentLoaded'de hem de doğrudan
   * çağrılabilir (script body'nin sonundaysa).
   */
  function initTheme() {
    const { base, dark } = getTheme();
    applyTheme(base, dark);

    const accent = getAccentColor();
    applyAccentColor(accent);

    const density = getTableDensity();
    applyTableDensity(density);
  }

  // ==========================================================
  // 6. STORAGE EVENT İLE SEKMELER ARASI SENKRONİZASYON
  // ==========================================================

  /**
   * Farklı sekmede tema değiştiğinde storage event'i tetiklenir.
   * Bu event'i yakalayıp anlık olarak temayı güncelleriz.
   */
  function handleStorageChange(event) {
    if (!event.key) {
      // Tüm storage temizlenmiş olabilir, yeniden yükle
      initTheme();
      return;
    }

    // Sadece bizim anahtarlarımızı dinle
    const ourKeys = Object.values(KEYS);
    if (!ourKeys.includes(event.key)) return;

    // Değişen anahtara göre güncelle
    switch (event.key) {
      case KEYS.THEME_BASE:
      case KEYS.THEME_DARK: {
        const { base, dark } = getTheme();
        applyTheme(base, dark);
        break;
      }
      case KEYS.ACCENT_COLOR: {
        const color = getAccentColor();
        applyAccentColor(color);
        break;
      }
      case KEYS.TABLE_DENSITY: {
        const density = getTableDensity();
        applyTableDensity(density);
        break;
      }
    }
  }

  // ==========================================================
  // 7. DIŞA AÇIK API
  // ==========================================================

  /**
   * Global tema API'si.
   * Diğer script'ler (settings.js, index.js vb.) bu API
   * üzerinden temayı değiştirebilir.
   *
   * @example
   *   window.PlatarTheme.setTheme("ocean", true);
   *   window.PlatarTheme.setAccentColor("purple");
   *   window.PlatarTheme.setDensity("compact");
   */
  window.PlatarTheme = {
    /** Mevcut tema yapılandırmasını döndürür */
    getTheme,

    /** Mevcut vurgu rengini döndürür */
    getAccentColor,

    /** Mevcut tablo yoğunluğunu döndürür */
    getTableDensity,

    /** Tema değiştirir (base: tema adı, dark: koyu varyant) */
    setTheme: function (base, dark) {
      applyTheme(base, dark);
    },

    /** Vurgu rengi değiştirir */
    setAccentColor: function (color) {
      applyAccentColor(color);
    },

    /** Tablo yoğunluğu değiştirir */
    setDensity: function (density) {
      applyTableDensity(density);
    },

    /** Tüm ayarları yeniden yükler (storage'dan) */
    reload: initTheme,

    /** localStorage anahtarları (test/debug için) */
    KEYS: { ...KEYS },
  };

  // ==========================================================
  // 8. OLAY DİNLEYİCİLERİ
  // ==========================================================

  // Sayfa yüklendiğinde temayı uygula
  document.addEventListener("turbo:load", initTheme);

  // Sekmeler arası senkronizasyon (storage event)
  window.addEventListener("storage", handleStorageChange);

  // ==========================================================
  // 9. FOCUS / VISIBILITY İLE SENKRONİZASYON (GARANTİ)
  // ==========================================================

  /**
   * Sayfa odağa geldiğinde veya görünür hale geldiğinde
   * localStorage'ı kontrol eder ve gerekirse temayı günceller.
   * Bu, storage event'inin tetiklenmediği durumlar için
   * yedek mekanizmadır.
   */
  function syncOnFocus() {
    // localStorage'daki son değerleri al
    const storedBase = getStoredValue(KEYS.THEME_BASE, "light");
    const storedDark = getStoredValue(KEYS.THEME_DARK) === "1";
    const storedAccent = getStoredValue(KEYS.ACCENT_COLOR, "blue");
    const storedDensity = getStoredValue(KEYS.TABLE_DENSITY, "normal");

    // Mevcut HTML attribute'larını al
    const currentTheme = html.dataset.theme;
    const currentAccent = html.dataset.accent;
    const currentDensity = html.dataset.density;

    // Beklenen theme değerini hesapla
    let expectedTheme;
    if (storedBase === "light") {
      expectedTheme = storedDark ? "dark" : "light";
    } else {
      expectedTheme = storedDark ? `${storedBase}-dark` : storedBase;
    }

    // Tema güncel değilse düzelt
    if (expectedTheme !== currentTheme) {
      applyTheme(storedBase, storedDark);
    }
    if (storedAccent !== currentAccent) {
      applyAccentColor(storedAccent);
    }
    if (storedDensity !== currentDensity) {
      applyTableDensity(storedDensity);
    }
  }

  // Sayfa görünür hale geldiğinde senkronize et
  document.addEventListener("visibilitychange", function () {
    if (!document.hidden) {
      syncOnFocus();
    }
  });

  // Pencere odağa geldiğinde senkronize et
  window.addEventListener("focus", syncOnFocus);
})();
