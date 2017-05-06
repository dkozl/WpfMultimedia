using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMultimedia.Webcam
{
    public class VideoInputResolutionComparer:IEqualityComparer<VideoInputResolution>, IComparer<VideoInputResolution>
    {
        bool IEqualityComparer<VideoInputResolution>.Equals(VideoInputResolution x, VideoInputResolution y)
        {
            if (x != null && y != null)
            {
                if (!Object.ReferenceEquals(x, y))
                    return x.Equals(y);
                return true;
            }
            else
                return x == null && y == null;
        }

        int IEqualityComparer<VideoInputResolution>.GetHashCode(VideoInputResolution obj)
        {
            return obj.GetHashCode();
        }

        int IComparer<VideoInputResolution>.Compare(VideoInputResolution x, VideoInputResolution y)
        {
            if (x != null && y != null)
            {
                var result = x.Width - y.Width;
                if (result == 0)
                {
                    result = x.Height - y.Height;
                    if (result == 0)
                        result = x.BitCount - y.BitCount;
                }
                return result;
            }
            return 0;
        }
    }
}
