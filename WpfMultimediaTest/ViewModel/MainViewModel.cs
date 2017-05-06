using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows;
using WpfMultimedia;
using WpfMultimedia.Webcam;

namespace WpfMultimediaTest.ViewModel
{
    public class MainViewModel : MVVM.ObservableObject
    {
        private readonly VideoCapture _webCam;

        private bool _canCapture = true;

        private CaptureFrameEventArgs _currentFrame;

        public MainViewModel()
        {
            _webCam = new VideoCapture(DeviceManager.GetVideoInputDevices().First(), new VideoInputResolutionSelector { MinBitCount = 24 });
            _webCam.CaptureFrame += OnFrameCaptured;
            UpdateCaptureState(_canCapture);
        }

        private void OnFrameCaptured(object sender, CaptureFrameEventArgs e)
        {
            _currentFrame = e;
            Application.Current.Dispatcher.BeginInvoke((Action)(() => OnPropertyChanged("Frame")), null);
        }

        public CaptureFrameEventArgs Frame
        {
            get { return _currentFrame; }
        }

        public string DeviceName
        {
            get { return _webCam.DeviceName; }
        }

        public int Width
        {
            get { return _webCam.Width; }
        }

        public int Height
        {
            get { return _webCam.Height; }
        }

        private bool UpdateCaptureState(bool status)
        {
            if (status)
                return _webCam.Start();
            else
                return _webCam.Pause();
        }

        public bool CanCapture
        {
            get { return _canCapture; }
            set
            {
                if (_canCapture != value)
                {
                    _canCapture = value;
                    OnPropertyChanged("CanCapture");
                    UpdateCaptureState(_canCapture);
                }
            }
        }

    }
}