// settings-service.js - Direct backend API integration for locations and cameras
window.CameraService = (function() {
    
    // API endpoints mapping directly to CameraController
    const API = {
        addLocation: '/Camera/AddLocationApi',
        updateLocation: '/Camera/UpdateLocationApi',
        deleteLocation: '/Camera/DeleteLocationApi',
        addCamera: '/Camera/AddCameraApi',
        updateCamera: '/Camera/UpdateCameraApi',
        deleteCamera: '/Camera/DeleteCameraApi'
    };

    // Helper method to make JSON POST requests
    async function request(url, body) {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(body)
        });
        
        if (!response.ok) {
            const errText = await response.text();
            throw new Error(errText || 'Sunucu hatası oluştu.');
        }

        const data = await response.json();
        if (data.success === false) {
            throw new Error(data.message || 'İşlem başarısız oldu.');
        }
        return data;
    }

    // ========== LOKASYON CRUD ==========
    async function addLocation(name, description) {
        const result = await request(API.addLocation, { name, description });
        return result.id; // Yeni oluşturulan lokasyonun Guid değerini döner
    }

    async function updateLocation(id, name, description) {
        await request(API.updateLocation, { id, name, description });
    }

    async function deleteLocation(id) {
        await request(API.deleteLocation, { id });
    }

    // ========== KAMERA CRUD ==========
    async function addCamera(locationId, cameraData) {
        const payload = {
            locationId: locationId,
            name: cameraData.name,
            ipAddress: cameraData.ipAddress,
            port: parseInt(cameraData.port, 10) || 80,
            username: cameraData.username || '',
            password: cameraData.password || '',
            streamChannel: parseInt(cameraData.streamChannel, 10) || 101
        };
        const result = await request(API.addCamera, payload);
        return result.id;
    }

    async function updateCamera(cameraId, cameraData) {
        const payload = {
            id: cameraId,
            locationId: cameraData.locationId,
            name: cameraData.name,
            ipAddress: cameraData.ipAddress,
            port: parseInt(cameraData.port, 10) || 80,
            username: cameraData.username || '',
            password: cameraData.password || '',
            streamChannel: parseInt(cameraData.streamChannel, 10) || 101
        };
        await request(API.updateCamera, payload);
    }

    async function deleteCamera(cameraId) {
        await request(API.deleteCamera, { id: cameraId });
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