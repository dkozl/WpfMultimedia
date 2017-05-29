using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimedia.Webcam.Business
{
    public class DefaultVideoResolutionSelector : IVideoResolutionSeletor
    {
        public int? MinWidth { get; set; }

        public int? MaxWidth { get; set; }

        public int? MinHeight { get; set; }

        public int? MaxHeight { get; set; }

        public int? MinBitCount { get; set; }

        public int? MaxBitCount { get; set; }

        public double? MinProportion { get; set; }

        public double? MaxProportion { get; set; }

        public bool IsMatch(int width, int height, int bitCount)
        {
            bool result = true;
            if (this.MinWidth.HasValue)
                result &= width >= this.MinWidth.Value;
            if (this.MaxWidth.HasValue)
                result &= width <= this.MaxWidth.Value;
            if (this.MinHeight.HasValue)
                result &= height >= this.MinHeight.Value;
            if (this.MaxHeight.HasValue)
                result &= height <= this.MaxHeight.Value;
            if (this.MinBitCount.HasValue)
                result &= bitCount >= this.MinBitCount.Value;
            if (this.MaxBitCount.HasValue)
                result &= bitCount <= this.MaxBitCount.Value;

            var widthToHeightRatio = (double)width / (double)height;

            if (this.MinProportion.HasValue)
                result &= widthToHeightRatio >= this.MinProportion.Value;
            if (this.MaxProportion.HasValue)
                result &= widthToHeightRatio <= this.MaxProportion.Value;
            return result;
        }
    }
}
