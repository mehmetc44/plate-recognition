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
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const email = emailInput.value.trim();
        const password = passwordInput.value.trim();

        // Hata mesajını sıfırla
        hideError();

        // Validasyon
        if (!email) {
            showError('E-posta veya kullanıcı adı gerekli.');
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

        // Set Loading state
        loginBtn.classList.add('loading');
        const spanEl = loginBtn.querySelector('span');
        const iconEl = loginBtn.querySelector('i');
        
        const origSpan = spanEl ? spanEl.textContent : 'Giriş Yap';
        const origIconClass = iconEl ? iconEl.className : '';
        
        if (spanEl) spanEl.textContent = 'Giriş yapılıyor...';
        if (iconEl) iconEl.className = 'fas fa-spinner spinner-icon';

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            const data = await response.json();

            if (response.ok && data.success) {
                // Kullanıcı bilgilerini localStorage'a kaydet (UI için)
                try {
                    localStorage.setItem('platar_user', JSON.stringify({
                        email: email,
                        name: email.includes('@') ? email.split('@')[0] : email,
                        role: 'Yönetici',
                        loggedIn: true,
                        loginTime: new Date().toISOString()
                    }));
                } catch (e) { /* ignore */ }

                // Ana sayfaya yönlendir
                window.location.href = '/';
            } else {
                showError(data.message || 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.');
                resetLoadingState(origSpan, origIconClass);
            }
        } catch (err) {
            showError('Sunucuya bağlanılamadı. Lütfen internet bağlantınızı kontrol edin.');
            resetLoadingState(origSpan, origIconClass);
        }
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

    function resetLoadingState(spanText, iconClass) {
        loginBtn.classList.remove('loading');
        const spanEl = loginBtn.querySelector('span');
        const iconEl = loginBtn.querySelector('i');
        if (spanEl) spanEl.textContent = spanText;
        if (iconEl && iconClass) iconEl.className = iconClass;
    }
})();
