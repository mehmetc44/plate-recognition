// ==========================================
// LOGIN SAYFASI - Giriş İşlemleri
// ==========================================
(function () {

    const form = document.getElementById('loginForm');
    if (!form) return;
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    const passwordToggle = document.getElementById('passwordToggle');
    const loginBtn = document.getElementById('loginBtn');
    const errorMsg = document.createElement('div');

    // Hata mesajı elementi
    errorMsg.className = 'login-error';
    errorMsg.innerHTML = '<i class="fas fa-exclamation-circle"></i> <span></span>';
    form.insertBefore(errorMsg, form.firstChild);

    // ==========================================================
    // 1. ŞİFRE GÖSTER/GİZLE
    // ==========================================================
    passwordToggle.addEventListener('click', function () {
        const isPassword = passwordInput.type === 'password';
        passwordInput.type = isPassword ? 'text' : 'password';
        this.querySelector('i').className = isPassword ? 'fas fa-eye-slash' : 'fas fa-eye';
    });

    // ==========================================================
    // 2. FORM VALİDASYON & GİRİŞ
    // ==========================================================
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const email = emailInput.value.trim();
        const password = passwordInput.value.trim();

        // Hata mesajını sıfırla
        hideError();

        // Validasyon
        if (!email) {
            showError('E-posta adresi gerekli.');
            emailInput.focus();
            return;
        }

        if (!isValidEmail(email)) {
            showError('Geçerli bir e-posta adresi girin.');
            emailInput.focus();
            return;
        }

        if (!password) {
            showError('Şifre gerekli.');
            passwordInput.focus();
            return;
        }

        if (password.length < 3) {
            showError('Şifre en az 3 karakter olmalıdır.');
            passwordInput.focus();
            return;
        }

        // Başarılı giriş simülasyonu
        loginBtn.classList.add('loading');
        loginBtn.querySelector('span').textContent = 'Giriş yapılıyor...';
        loginBtn.querySelector('i').className = 'fas fa-spinner';

        setTimeout(function () {
            // Kullanıcı bilgilerini localStorage'a kaydet
            try {
                localStorage.setItem('platar_user', JSON.stringify({
                    email: email,
                    name: email === 'admin@platar.com' ? 'Admin' : email.split('@')[0],
                    role: 'Yönetici',
                    loggedIn: true,
                    loginTime: new Date().toISOString()
                }));
            } catch (e) { /* ignore */ }

            // Ana sayfaya yönlendir
            window.location.href = 'index.html';
        }, 1200);
    });

    // ==========================================================
    // 3. YARDIMCI FONKSİYONLAR
    // ==========================================================
    function showError(message) {
        errorMsg.querySelector('span').textContent = message;
        errorMsg.classList.add('show');
        // 4 saniye sonra otomatik gizle
        setTimeout(hideError, 4000);
    }

    function hideError() {
        errorMsg.classList.remove('show');
    }

    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    // ==========================================================
    // 4. EĞER ZATEN GİRİŞ YAPILMIŞSA YÖNLENDİR
    // ==========================================================
    try {
        const userData = localStorage.getItem('platar_user');
        if (userData) {
            const user = JSON.parse(userData);
            if (user.loggedIn) {
                // Zaten giriş yapılmış, ana sayfaya yönlendir
                window.location.href = 'index.html';
            }
        }
    } catch (e) { /* ignore */ }
})();
