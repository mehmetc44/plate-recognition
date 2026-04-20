using System;
using OpenCvSharp;
using System.Collections.Concurrent;
using System.Net;
using PlakaTanima.Application.Abstract.Services;
using System.Runtime.CompilerServices;
namespace PlakaTanima.Infrastructure.Services;

public class CameraService : ICameraService
{

   // Verilen linki buraya çakıyoruz
    private const string RtspUrl = "rtsp://admin:kutup12.@10.10.155.77:554/Streaming/Channels/101";

    public async IAsyncEnumerable<byte[]> GetLiveStreamAsync([EnumeratorCancellation] CancellationToken ct)
    {
        using var capture = new VideoCapture(RtspUrl);
        
        // Bağlantı hızını artırmak için buffer ayarı (opsiyonel)
        capture.Set(VideoCaptureProperties.BufferSize, 3); 

        if (!capture.IsOpened())
        {
            throw new Exception("Kameraya bağlanılamadı! Linki veya ağını kontrol et.");
        }

        using var frame = new Mat();
        while (!ct.IsCancellationRequested)
        {
            if (capture.Read(frame) && !frame.Empty())
            {
                // Kareyi JPEG byte dizisine çeviriyoruz
                yield return frame.ToBytes(".jpg");
            }

            // İşlemciyi (CPU) ağlatmamak için ufak bir bekleme (yaklaşık 20-25 FPS)
            await Task.Delay(40, ct); 
        }
    }
}