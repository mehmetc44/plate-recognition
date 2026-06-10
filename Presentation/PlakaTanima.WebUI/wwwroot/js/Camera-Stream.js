const MEDIA_MTX = "http://localhost:8888";

// aktif stream cache
const hlsInstances = {};

/**
 * Sadece HLS stream bağlar
 * @param {HTMLVideoElement} videoEl
 * @param {string} streamId
 */
function connectHlsStream(videoEl, streamId) {

    if (!videoEl || !streamId) return;

    const url = `${MEDIA_MTX}/${streamId}/index.m3u8`;

    // eski stream varsa temizle
    if (hlsInstances[streamId]) {
        hlsInstances[streamId].destroy();
        delete hlsInstances[streamId];
    }

    if (Hls.isSupported()) {

        const hls = new Hls({
            lowLatencyMode: true
        });

        hls.loadSource(url);
        hls.attachMedia(videoEl);

        hls.on(Hls.Events.MANIFEST_PARSED, () => {
            videoEl.play();
        });

        hls.on(Hls.Events.ERROR, () => {
            console.warn("HLS error:", streamId);
        });

        hlsInstances[streamId] = hls;

    } else {
        videoEl.src = url;

        videoEl.onloadedmetadata = () => {
            videoEl.play();
        };
    }
}

/**
 * Stream temizleme (opsiyonel ama önemli)
 */
function disconnectHlsStream(streamId) {
    if (hlsInstances[streamId]) {
        hlsInstances[streamId].destroy();
        delete hlsInstances[streamId];
    }
}

// global export
window.connectHlsStream = connectHlsStream;
window.disconnectHlsStream = disconnectHlsStream;