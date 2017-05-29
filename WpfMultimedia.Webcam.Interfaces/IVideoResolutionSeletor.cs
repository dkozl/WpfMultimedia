namespace WpfMultimedia.Webcam.Interfaces
{
    public interface IVideoResolutionSeletor
    {
        bool IsMatch(int width, int height, int bitCount);
    }
}
