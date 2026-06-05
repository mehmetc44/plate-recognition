// cameraService.js - Tüm AJAX istekleri (API endpoint'leri)
window.CameraService = (function() {
    const BASE = '/Settings';

    // ----- LOKASYON CRUD (API) -----
    async function addLocation(name, description) {
        const res = await fetch(`${BASE}/AddLocationApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, description })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Lokasyon eklenemedi');
        return data.id;
    }

    async function updateLocation(id, name, description) {
        const res = await fetch(`${BASE}/UpdateLocationApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: id, Name: name, Description: description })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Lokasyon güncellenemedi');
    }

    async function deleteLocation(id) {
        const res = await fetch(`${BASE}/DeleteLocationApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: id })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Lokasyon silinemedi');
    }

    // ----- KAMERA CRUD (API) -----
    async function addCamera(locationId, cameraData) {
        const res = await fetch(`${BASE}/AddCameraApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ locationId, ...cameraData })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Kamera eklenemedi');
        return data.id;
    }

    async function updateCamera(cameraId, cameraData) {
        const res = await fetch(`${BASE}/UpdateCameraApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: cameraId, ...cameraData })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Kamera güncellenemedi');
    }

    async function deleteCamera(cameraId) {
        const res = await fetch(`${BASE}/DeleteCameraApi`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Id: cameraId })
        });
        const data = await res.json();
        if (!data.success) throw new Error(data.message || 'Kamera silinemedi');
    }

    return {
        addLocation,
        updateLocation,
        deleteLocation,
        addCamera,
        updateCamera,
        deleteCamera
    };
})();