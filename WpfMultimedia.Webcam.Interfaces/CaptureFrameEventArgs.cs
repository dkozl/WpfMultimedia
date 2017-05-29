using System;
using System.Windows.Media.Imaging;

namespace WpfMultimedia.Webcam.Interfaces
{
    public class CaptureFrameEventArgs : EventArgs
    {
        private readonly string _deviceName;

        private readonly BitmapSource _image;

        private readonly DateTime _timeStamp;

        public CaptureFrameEventArgs(string deviceName, BitmapSource image)
        {
            _deviceName = deviceName;
            _timeStamp = DateTime.Now;
            _image = image;
        }

        public string DeviceName { get { return _deviceName; } }

        public BitmapSource Frame { get { return _image; } }

        public DateTime TimeStamp { get { return _timeStamp; } }

        public int Width { get { return _image.PixelWidth; } }

        public int Height { get { return _image.PixelHeight; } }
    }
}