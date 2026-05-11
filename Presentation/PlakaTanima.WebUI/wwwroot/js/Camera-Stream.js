// =======================
// CAMERA LOADER (HLS ONLY)
// =======================
// =======================
// MEDIA MTX
// =======================
const MEDIA_MTX = "http://localhost:8888";

// stream cache
const hlsInstances = {};
function loadCameraToSlot(slot, camName, streamId) {

    // cleanup old stream
    if (slot.dataset.stream && hlsInstances[slot.dataset.stream]) {
        hlsInstances[slot.dataset.stream].destroy();
        delete hlsInstances[slot.dataset.stream];
    }

    slot.dataset.stream = streamId;

    slot.innerHTML = "";

    slot.style.position = "relative";
    slot.style.overflow = "hidden";
    slot.style.background = "#000";

    // overlay
    const info = document.createElement("div");
    info.style.cssText =
        "position:absolute;top:10px;left:10px;color:white;z-index:10;font-weight:bold;text-shadow:1px 1px 2px black;";
    info.innerText = `${camName}`;
    slot.appendChild(info);

    // status
    const status = document.createElement("div");
    status.style.cssText =
        "position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);color:#777;";
    status.innerText = "Bağlanıyor...";
    slot.appendChild(status);

    // video
    const video = document.createElement("video");
    video.autoplay = true;
    video.muted = true;
    video.playsInline = true;
    video.style.width = "100%";
    video.style.height = "100%";
    video.style.objectFit = "cover";

    slot.appendChild(video);

    const url = `${MEDIA_MTX}/${streamId}/index.m3u8`;

    if (Hls.isSupported()) {

        const hls = new Hls({
            lowLatencyMode: true
        });

        hls.loadSource(url);
        hls.attachMedia(video);

        hls.on(Hls.Events.MANIFEST_PARSED, () => {
            status.style.display = "none";
            video.play();
        });

        hls.on(Hls.Events.ERROR, () => {
            status.innerText = "Sinyal Yok";
        });

        hlsInstances[streamId] = hls;

    } else {
        video.src = url;
        video.onloadedmetadata = () => {
            status.style.display = "none";
            video.play();
        };
    }
}
window.loadCameraToSlot = loadCameraToSlot;