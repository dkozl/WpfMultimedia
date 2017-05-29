using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfMultimedia.Webcam.Interfaces;

namespace WpfMultimedia.Webcam
{
    public sealed class DirectShowVideoCapture : ISampleGrabberCB, IVideoCapture
    {
        #region Private static methods

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, [MarshalAs(UnmanagedType.U4)] int length);

        #endregion

        #region Private fields

        private readonly Dispatcher _dispatcher;

        private readonly string _deviceName;

        private readonly IMediaEventEx _mediaEvent;

        private readonly IMediaControl _mediaControl;

        private readonly ISampleGrabber _sampleGrabber;

        private readonly System.Timers.Timer _timer;

        private CameraStatus _cameraStatus = CameraStatus.Stop;

        private readonly Object _cameraStatusLock = new Object();

        private readonly List<IVideoResolution> _allResolutions;

        private readonly IVideoResolution[] _matchingResolutions;

        private readonly IVideoResolution _resolution;

        private bool _disposed;

        #endregion

        #region Constructor

        public DirectShowVideoCapture(DsDevice device, IVideoResolutionSeletor resolutionSelector)
        {
            if (device == null)
                throw new ArgumentNullException("device", "Device must not be null");

            _dispatcher = Dispatcher.CurrentDispatcher;

            _deviceName = device.Name;

            ICaptureGraphBuilder2 graphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());

            IGraphBuilder graph = (IGraphBuilder)(new FilterGraphNoThread());

            Marshal.ThrowExceptionForHR(graphBuilder.SetFiltergraph(graph));

            IBaseFilter filter;
            IFilterGraph2 filterGraph = (IFilterGraph2)graph;
            DsError.ThrowExceptionForHR(filterGraph.AddSourceFilterForMoniker(device.Mon, null, device.Name, out filter));

            object streamConfig;

            ICaptureGraphBuilder2 captureGraphBuilder = (ICaptureGraphBuilder2)graphBuilder;
            DsError.ThrowExceptionForHR(captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, filter, typeof(IAMStreamConfig).GUID, out streamConfig));
            var videoStreamConfig = streamConfig as IAMStreamConfig;

            int capCount, capSize;
            videoStreamConfig.GetNumberOfCapabilities(out capCount, out capSize);

            IntPtr taskMemPointer = Marshal.AllocCoTaskMem(capSize);

            AMMediaType pmtConfig = null;

            _allResolutions = new List<IVideoResolution>(capSize);
            for (int iFormat = 0; iFormat < capCount; iFormat++)
            {
                IntPtr ptr = IntPtr.Zero;

                videoStreamConfig.GetStreamCaps(iFormat, out pmtConfig, taskMemPointer);
                var v = (VideoInfoHeader)Marshal.PtrToStructure(pmtConfig.formatPtr, typeof(VideoInfoHeader));
                var bmiHeader = v.BmiHeader;
                _allResolutions.Add(new VideoResolution(bmiHeader.Width, bmiHeader.Height, (int)bmiHeader.BitCount));
            }
            _allResolutions.Sort(new VideoResolutionComparer());
            _matchingResolutions = _allResolutions.Where(r => resolutionSelector == null || resolutionSelector.IsMatch(r.Width, r.Height, r.BitCount)).ToArray();
            _resolution = _matchingResolutions.Last();

            Marshal.FreeCoTaskMem(taskMemPointer);
            DsUtils.FreeAMMediaType(pmtConfig);

            AMMediaType media;
            DsError.ThrowExceptionForHR(videoStreamConfig.GetFormat(out media));

            var videoInfo = new VideoInfoHeader();
            Marshal.PtrToStructure(media.formatPtr, videoInfo);

            videoInfo.BmiHeader.Width = _resolution.Width;
            videoInfo.BmiHeader.Height = _resolution.Height;
            videoInfo.BmiHeader.BitCount = (short)_resolution.BitCount;

            Marshal.StructureToPtr(videoInfo, media.formatPtr, false);

            DsError.ThrowExceptionForHR(videoStreamConfig.SetFormat(media));
            DsUtils.FreeAMMediaType(media);

            //sample grabber
            var mediaType = new AMMediaType
            {
                majorType = MediaType.Video,
                subType = MediaSubType.RGB24,
            };

            _sampleGrabber = (ISampleGrabber)(new SampleGrabber());
            Marshal.ThrowExceptionForHR(_sampleGrabber.SetMediaType(mediaType));
            DsUtils.FreeAMMediaType(mediaType);
            Marshal.ThrowExceptionForHR(_sampleGrabber.SetOneShot(false));
            Marshal.ThrowExceptionForHR(_sampleGrabber.SetBufferSamples(true));
            Marshal.ThrowExceptionForHR(_sampleGrabber.SetCallback(this, 1));
            IBaseFilter grabberFilter = (IBaseFilter)_sampleGrabber;
            Marshal.ThrowExceptionForHR(graph.AddFilter(grabberFilter, "SampleGrabber"));

            IBaseFilter nullRenderer = (IBaseFilter)(new NullRenderer());
            Marshal.ThrowExceptionForHR(graph.AddFilter(nullRenderer, "Null Filter"));
            Marshal.ThrowExceptionForHR(graphBuilder.RenderStream(null, MediaType.Video, filter, null, grabberFilter));

            _mediaEvent = graph as IMediaEventEx;
            _mediaControl = graph as IMediaControl;

            Marshal.ReleaseComObject(graphBuilder);

            _timer = new Timer(50);
            _timer.Elapsed += TimerElapsed;
        }

        #endregion

        #region Device events timer processing

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_mediaEvent != null)
            {
                IntPtr param1;
                IntPtr param2;
                EventCode code;

                while (_mediaEvent.GetEvent(out code, out param1, out param2, 0) == 0)
                {
                    _mediaEvent.FreeEventParams(code, param1, param2);
                }
            }
        }

        #endregion

        #region ISampleGrabberCB

        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            using (var bitmapFrame = new Bitmap(_resolution.Width, _resolution.Height, PixelFormat.Format24bppRgb))
            {
                var bmpData = bitmapFrame.LockBits(new Rectangle(0, 0, bitmapFrame.Width, bitmapFrame.Height), ImageLockMode.ReadWrite, bitmapFrame.PixelFormat);
                CopyMemory(bmpData.Scan0, pBuffer, BufferLen);
                bitmapFrame.UnlockBits(bmpData);
                bitmapFrame.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmapFrame.Save(memory, ImageFormat.Bmp);
                    memory.Position = 0;
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    OnCaptureFrame(bitmapImage);
                }
            }
            return 0;
        }

        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch { }
            finally
            {
                Marshal.ReleaseComObject(pSample);
            }
            return 0;
        }

        #endregion

        #region Events

        public event EventHandler<CaptureFrameEventArgs> CaptureFrame;

        private void OnCaptureFrame(BitmapSource img)
        {
            if (img != null)
            {
                var handler = CaptureFrame;
                if (handler != null)
                {
                    var args = new CaptureFrameEventArgs(_deviceName, img);
                    _dispatcher.BeginInvoke(handler, DispatcherPriority.Normal, new object[] { this, args });
                }
            }
        }

        #endregion

        #region Public properties

        public IVideoResolution Resolution
        {
            get { return _resolution; }
        }

        #endregion

        #region Private methods

        private DispatcherOperation BeginInvoke(Action action, DispatcherPriority priority)
        {
            return _dispatcher.BeginInvoke(action, priority, null);
        }

        private bool ChangeCameraStatus(CameraStatus newStatus)
        {
            bool valueChanged = false;
            lock (_cameraStatusLock)
            {
                if (_cameraStatus != newStatus)
                {
                    bool canChange = false;
                    switch (newStatus)
                    {
                        case CameraStatus.Capture:
                            canChange = _cameraStatus == CameraStatus.Pause || _cameraStatus == CameraStatus.Stop;
                            break;
                        case CameraStatus.Pause:
                            canChange = _cameraStatus == CameraStatus.Capture;
                            break;
                        case CameraStatus.Stop:
                            canChange = _cameraStatus == CameraStatus.Capture || _cameraStatus == CameraStatus.Pause;
                            break;
                    }
                    if (canChange)
                    {
                        _cameraStatus = newStatus;
                        valueChanged = true;
                    }
                }
            }
            return valueChanged;
        }

        #endregion

        #region Public methods

        public CameraStatus Start()
        {
            if (ChangeCameraStatus(CameraStatus.Capture))
            {
                _mediaControl.Run();
                _timer.Start();
            }
            return _cameraStatus;
        }

        public CameraStatus Pause()
        {
            if (ChangeCameraStatus(CameraStatus.Pause))
            {
                _timer.Stop();
                _mediaControl.Pause();
            }
            return _cameraStatus;
        }

        public CameraStatus Stop(bool forceStop)
        {
            if (ChangeCameraStatus(CameraStatus.Stop))
            {
                _timer.Stop();
                if (forceStop)
                    _mediaControl.Stop();
                else
                    _mediaControl.StopWhenReady();
            }
            return _cameraStatus;
        }

        #endregion

        #region IDisposable

        private void Dispose(bool isDisposing)
        {
            if (!_disposed)
            {
                Stop(true);
                if (isDisposing)
                {
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

}