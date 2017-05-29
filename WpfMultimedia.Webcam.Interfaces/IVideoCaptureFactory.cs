namespace WpfMultimedia.Webcam.Interfaces
{
    public interface IVideoCaptureFactory
    {
        IVideoCapture CreateVideoCapture(string deviceName, IVideoResolutionSeletor selector);
    }
}
