using WpfMultimedia.Webcam.Interfaces;
using System.Linq;

namespace WpfMultimedia.Webcam.Business
{
    public class DefaultVideoCaptureFactory : IVideoCaptureFactory
    {
        public IVideoCapture CreateVideoCapture(string deviceName, IVideoResolutionSeletor selector)
        {
            return new DirectShowVideoCapture(DeviceManager.GetVideoInputDevices().Single(d => d.Name.Equals(deviceName)), selector);
        }
    }
}