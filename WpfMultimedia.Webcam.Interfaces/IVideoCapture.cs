using System;

namespace WpfMultimedia.Webcam.Interfaces
{
    public interface IVideoCapture : IDisposable
    {
        CameraStatus Start();

        CameraStatus Pause();

        CameraStatus Stop(bool forceStop);

        IVideoResolution Resolution { get; }

        event EventHandler<CaptureFrameEventArgs> CaptureFrame;
    }
}