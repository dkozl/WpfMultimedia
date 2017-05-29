using System;
using System.Linq;
using WpfMultimedia;
using WpfMultimedia.Webcam.Business;
using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimediaTest.ViewModel
{
    public class MainViewModel : MVVM.ObservableObject
    {
        private readonly IVideoCapture _webCam;

        private bool _canCapture = true;

        private CaptureFrameEventArgs _currentFrame;

        public MainViewModel()
        {
            _webCam = (new DefaultVideoCaptureFactory()).CreateVideoCapture(DeviceManager.GetVideoInputDevices().First().Name, new DefaultVideoResolutionSelector { MaxWidth = 1600, MinBitCount = 24 });
            _webCam.CaptureFrame += OnFrameCaptured;
            UpdateCaptureState(_canCapture);
        }

        private void OnFrameCaptured(object sender, CaptureFrameEventArgs e)
        {
            this.Frame = e;
        }

        public CaptureFrameEventArgs Frame
        {
            get { return _currentFrame; }
            private set { UpdateProperty("Frame", value, ref _currentFrame); }
        }

        public string DeviceName
        {
            get { return String.Empty; }
        }

        public int Width
        {
            get { return _webCam.Resolution.Width; }
        }

        public int Height
        {
            get { return _webCam.Resolution.Height; }
        }

        private bool UpdateCaptureState(bool status)
        {
            if (status)
                return _webCam.Start() == CameraStatus.Capture;
            else
                return _webCam.Pause() == CameraStatus.Pause;
        }

        public bool CanCapture
        {
            get { return _canCapture; }
            set { UpdateProperty("CanCapture", value, ref _canCapture); }
        }

    }
}