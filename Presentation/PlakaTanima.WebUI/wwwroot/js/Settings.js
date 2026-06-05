// settings.js - Sadece sidebar geçişi ve yardımcı UI işleri
document.addEventListener('DOMContentLoaded', function() {
    // Sidebar sekme geçişi
    const menuItems = document.querySelectorAll('.menu-item');
    const panels = document.querySelectorAll('.settings-panel');
    function switchTab(tabId) {
        panels.forEach(p => p.classList.remove('active'));
        const activePanel = document.getElementById(`panel-${tabId}`);
        if (activePanel) activePanel.classList.add('active');
        menuItems.forEach(m => m.classList.remove('active'));
        const activeMenuItem = document.querySelector(`.menu-item[data-tab="${tabId}"]`);
        if (activeMenuItem) activeMenuItem.classList.add('active');
    }
    menuItems.forEach(item => {
        item.addEventListener('click', () => switchTab(item.getAttribute('data-tab')));
    });
});