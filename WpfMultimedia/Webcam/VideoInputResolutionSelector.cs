using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimedia.Webcam
{
    public class VideoInputResolutionSelector : IVideoInputResolutionSeletor
    {
        public int? MinWidth { get; set; }

        public int? MaxWidth { get; set; }

        public int? MinHeight { get; set; }

        public int? MaxHeight { get; set; }

        public int? MinBitCount { get; set; }

        public int? MaxBitCount { get; set; }

        public double? MinProportion { get; set; }

        public double? MaxProportion { get; set; }

        public bool IsMatch(VideoInputResolution resolution)
        {
            if (resolution != null)
            {
                bool result = true;
                if (this.MinWidth.HasValue)
                    result &= resolution.Width >= this.MinWidth.Value;
                if (this.MaxWidth.HasValue)
                    result &= resolution.Width <= this.MaxWidth.Value;
                if (this.MinHeight.HasValue)
                    result &= resolution.Height >= this.MinHeight.Value;
                if (this.MaxHeight.HasValue)
                    result &= resolution.Height <= this.MaxHeight.Value;
                if (this.MinBitCount.HasValue)
                    result &= resolution.BitCount >= this.MinBitCount.Value;
                if (this.MaxBitCount.HasValue)
                    result &= resolution.BitCount <= this.MaxBitCount.Value;
                if (this.MinProportion.HasValue)
                    result &= resolution.WidthToHeightRatio >= this.MinProportion.Value;
                if (this.MaxProportion.HasValue)
                    result &= resolution.WidthToHeightRatio <= this.MaxProportion.Value;
                return result;
            }
            return false;
        }
    }
}
