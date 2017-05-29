using System;
using System.Collections.Generic;
using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimedia.Webcam
{
    internal class VideoResolutionComparer : IEqualityComparer<IVideoResolution>, IComparer<IVideoResolution>
    {
        bool IEqualityComparer<IVideoResolution>.Equals(IVideoResolution x, IVideoResolution y)
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

        int IEqualityComparer<IVideoResolution>.GetHashCode(IVideoResolution obj)
        {
            return obj.GetHashCode();
        }

        int IComparer<IVideoResolution>.Compare(IVideoResolution x, IVideoResolution y)
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