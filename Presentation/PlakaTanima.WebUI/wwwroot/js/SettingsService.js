// cameraService.js - Veri ve API işlemleri (mock + AJAX)
window.CameraService = (function() {
    // ========== MOCK SEED DATA ==========
    let locations = [
        { 
            id: 'loc1', 
            name: 'Ana Giriş', 
            description: 'Ana giriş kapısı', 
            cameras: [
                { id: 'cam1', name: 'Kamera 1', ip: '192.168.1.101', port: 80, username: 'admin', password: '123', status: 'online' },
                { id: 'cam2', name: 'Kamera 2', ip: '192.168.1.102', port: 80, username: 'admin', password: '123', status: 'offline' }
            ] 
        },
        { 
            id: 'loc2', 
            name: 'Otopark', 
            description: 'Açık otopark alanı', 
            cameras: [
                { id: 'cam3', name: 'Otopark Giriş', ip: '192.168.1.201', port: 80, username: 'admin', password: '123', status: 'online' }
            ] 
        }
    ];

    // Gerçek backend adresleri (projendeki controller action'larına göre düzenle)
    const API = {
        addLocation: '/Settings/AddLocation',
        updateLocation: '/Settings/UpdateLocation',
        deleteLocation: '/Settings/DeleteLocation',
        addCamera: '/Settings/AddCamera',
        updateCamera: '/Settings/UpdateCamera',
        deleteCamera: '/Settings/DeleteCamera'
    };

    // Backend hazır mı? (mock modu: false)
    let useBackend = false;

    // ========== YARDIMCI FONKSİYONLAR ==========
    function setUseBackend(value) {
        useBackend = value;
    }

    function getLocations() {
        return locations;
    }

    function setLocations(data) {
        locations = data;
    }

    // ========== LOKASYON CRUD ==========
    async function addLocation(name, description) {
        if (!useBackend) {
            // Mock: yeni lokasyon oluştur
            const newId = 'loc_' + Date.now();
            const newLoc = { id: newId, name, description: description || '', cameras: [] };
            locations.push(newLoc);
            return newId;
        }
        const response = await fetch(API.addLocation, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, description })
        });
        if (!response.ok) throw new Error('Lokasyon eklenemedi');
        return await response.json(); // id döner
    }

    async function updateLocation(id, name, description) {
        if (!useBackend) {
            const loc = locations.find(l => l.id === id);
            if (loc) {
                loc.name = name;
                loc.description = description;
            }
            return;
        }
        await fetch(`${API.updateLocation}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name, description })
        });
    }

    async function deleteLocation(id) {
        if (!useBackend) {
            locations = locations.filter(l => l.id !== id);
            return;
        }
        await fetch(`${API.deleteLocation}/${id}`, { method: 'DELETE' });
    }

    // ========== KAMERA CRUD ==========
    async function addCamera(locationId, cameraData) {
        if (!useBackend) {
            const loc = locations.find(l => l.id === locationId);
            if (loc) {
                const newId = 'cam_' + Date.now();
                const newCam = { id: newId, ...cameraData, status: cameraData.status || 'offline' };
                loc.cameras.push(newCam);
                return newId;
            }
            return null;
        }
        const response = await fetch(API.addCamera, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ locationId, ...cameraData })
        });
        if (!response.ok) throw new Error('Kamera eklenemedi');
        return await response.json();
    }

    async function updateCamera(locationId, cameraId, cameraData) {
        if (!useBackend) {
            const loc = locations.find(l => l.id === locationId);
            const cam = loc?.cameras.find(c => c.id === cameraId);
            if (cam) Object.assign(cam, cameraData);
            return;
        }
        await fetch(`${API.updateCamera}/${cameraId}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ locationId, ...cameraData })
        });
    }

    async function deleteCamera(locationId, cameraId) {
        if (!useBackend) {
            const loc = locations.find(l => l.id === locationId);
            if (loc) loc.cameras = loc.cameras.filter(c => c.id !== cameraId);
            return;
        }
        await fetch(`${API.deleteCamera}/${cameraId}`, { method: 'DELETE' });
    }

    // ========== SEED / RESET ==========
    function resetToSeed() {
        locations = JSON.parse(JSON.stringify([
            { 
                id: 'loc1', 
                name: 'Ana Giriş', 
                description: 'Ana giriş kapısı', 
                cameras: [
                    { id: 'cam1', name: 'Kamera 1', ip: '192.168.1.101', port: 80, username: 'admin', password: '123', status: 'online' },
                    { id: 'cam2', name: 'Kamera 2', ip: '192.168.1.102', port: 80, username: 'admin', password: '123', status: 'offline' }
                ] 
            },
            { 
                id: 'loc2', 
                name: 'Otopark', 
                description: 'Açık otopark alanı', 
                cameras: [
                    { id: 'cam3', name: 'Otopark Giriş', ip: '192.168.1.201', port: 80, username: 'admin', password: '123', status: 'online' }
                ] 
            }
        ]));
    }

    // Public API
    return {
        setUseBackend,
        getLocations,
        setLocations,
        addLocation,
        updateLocation,
        deleteLocation,
        addCamera,
        updateCamera,
        deleteCamera,
        resetToSeed
    };
})();