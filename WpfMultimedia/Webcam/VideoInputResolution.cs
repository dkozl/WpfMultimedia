using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectShowLib;

namespace WpfMultimedia.Webcam
{
    [DebuggerDisplay("{Width}x{Height} {BitCount}")]
    public class VideoInputResolution
    {
        private readonly int _width;

        private readonly int _height;

        private readonly int _bitCount;

        public VideoInputResolution(int width, int height, int bitCount)
        {
            _width = width;
            _height = height;
            _bitCount = bitCount;
        }

        internal VideoInputResolution(BitmapInfoHeader bitmapInfoHader) : this(bitmapInfoHader.Width, bitmapInfoHader.Height, bitmapInfoHader.BitCount) { }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public double WidthToHeightRatio
        {
            get { return (double)_width / (double)_height; }
        }

        public int BitCount
        {
            get { return _bitCount; }
        }

        public override string ToString()
        {
            return String.Format("{0}x{1} {2}", this.Width, this.Height, this.BitCount);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public bool Equals(VideoInputResolution obj)
        {
            return obj != null && obj.Width == this.Width && obj.Height == this.Height && obj.BitCount == this.BitCount;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as VideoInputResolution);
        }
    }
}