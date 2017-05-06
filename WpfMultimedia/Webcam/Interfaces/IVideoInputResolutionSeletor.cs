using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfMultimedia.Webcam.Interfaces
{
    public interface IVideoInputResolutionSeletor
    {
        bool IsMatch(VideoInputResolution resolution);
    }
}
