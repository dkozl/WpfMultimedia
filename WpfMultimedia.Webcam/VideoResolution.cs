using System.Diagnostics;
using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimedia.Webcam
{
    [DebuggerDisplay("{Width}x{Height} {BitCount}")]
    internal class VideoResolution : IVideoResolution
    {
        private readonly int _width;

        private readonly int _height;

        private readonly int _bitCount;

        public VideoResolution(int width, int height, int bitCount)
        {
            _width = width;
            _height = height;
            _bitCount = bitCount;
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int BitCount
        {
            get { return _bitCount; }
        }
    }
}
