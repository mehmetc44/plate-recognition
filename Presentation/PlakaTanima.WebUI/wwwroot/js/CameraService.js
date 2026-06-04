// cameraService.js - Sadece Location ekleme API çağrısı
window.CameraService = (function() {
    // Backend API endpoint'i (SettingsController'daki action)
    const API_ADD_LOCATION = '/Settings/AddLocation';

    async function addLocation(name, description) {
        const response = await fetch(API_ADD_LOCATION, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, description })
        });
        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Lokasyon eklenemedi: ${response.status} ${errorText}`);
        }
        const newId = await response.json();
        return newId;
    }

    return {
        addLocation
    };
})();