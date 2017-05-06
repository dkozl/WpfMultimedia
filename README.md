# Multimedia library for WPF

**Currently supports only webcam**

Simply create `VideoCapture` class with selected video input device, optionally specify maximum frame size, subscribe to `CaptureFrame` event and start receiving frames from camera device

```
using WpfMultimedia;
using WpfMultimedia.Webcam;

var _webCam = new VideoCapture(DeviceManager.GetVideoInputDevices().First(), null);
_webCam.CaptureFrame += OnFrameCaptured;
_webCam.Start();

...

private void OnFrameCaptured(object sender, CaptureFrameEventArgs e)
{
    //latest BitmapImage from camera is in e.Frame
    BitmapImage _currentFrame = e.Frame
    ....
}
```
