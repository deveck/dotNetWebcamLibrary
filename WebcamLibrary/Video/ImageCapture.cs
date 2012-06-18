/* 
 * @author: deveck.net
 * 
 * This software is protected by Ms-PL 
 * see http://www.microsoft.com/opensource/licenses.mspx#Ms-PL or license.txt
 */

using System;
using System.Collections.Generic;
using System.Text;
using DirectShowLib;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using DevEck.Devices.Common;
using System.Drawing.Imaging;

namespace DevEck.Devices.Video
{
    /// <summary>
    /// Captures single images from the given device
    /// </summary>
    public class ImageCapture : ISampleGrabberCB, IDisposable
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        /// <summary>
        /// Device to capture from
        /// </summary>
        private Device _device;

        /// <summary>
        /// Graph builder interface
        /// </summary>
        private IFilterGraph2 _filterGraph = null;

        /// <summary>
        /// Controls the flow of data through the filter graph
        /// </summary>
        private IMediaControl _mediaCtrl = null;

        /// <summary>
        /// Indicates if the capture device is captureing ;)
        /// </summary>
        private bool _running = false;

        /// <summary>
        /// Memory that is allocated for the bitmaps
        /// </summary>
        private IntPtr _bitmapMemory = IntPtr.Zero;

        /// <summary>
        /// Dimensions
        /// </summary>
        private int _videoWidth = 0;
        private int _videoHeight = 0;
        private int _stride = 0;

        /// <summary>
        /// Determines if a picture should be captured
        /// </summary>
        private volatile bool _capturePicture = false;

        /// <summary>
        /// Is Set by BufferCB, to signal that the picture was successfully copied
        /// </summary>
        private ManualResetEvent _captureReady = new ManualResetEvent(false);

        /// <summary>
        /// Retrieves the current Image
        /// This method is NOT THREAD SAFE
        /// Synchronization need to be done outside
        /// The returned object need to be disposed
        /// </summary>
        public ComBitmap Image
        {
            get
            {
                _captureReady.Reset();
                _bitmapMemory = Marshal.AllocCoTaskMem(_stride * _videoHeight);
                _capturePicture = true;

                _captureReady.WaitOne();

                return new ComBitmap(_bitmapMemory, _videoWidth, _videoHeight, _stride, PixelFormat.Format24bppRgb);

            }
        }

        public int OriginalWidth
        {
            get { return _videoWidth; }
        }

        public int OriginalHeight
        {
            get { return _videoHeight; }
        }

        public ImageCapture(Device device)
        {
            _device = device;
        }

        /// <summary>
        /// Initializes the devices and prepares for capture
        /// </summary>
        public void Initialize()
        {
            _filterGraph = new FilterGraph() as IFilterGraph2;
            _mediaCtrl = _filterGraph as IMediaControl;

            ISampleGrabber sampGrabber = null;
            IBaseFilter capFilter = null;
            ICaptureGraphBuilder2 capGraph = null;
            int status;

            try
            {
                capGraph = new CaptureGraphBuilder2() as ICaptureGraphBuilder2;
                sampGrabber = new SampleGrabber() as ISampleGrabber;

                status = capGraph.SetFiltergraph(_filterGraph);
                DsError.ThrowExceptionForHR(status);

                //Adds the video device
                status = _filterGraph.AddSourceFilterForMoniker(_device.DsDevice.Mon, null, "Video input", out capFilter);
                DsError.ThrowExceptionForHR(status);

                IBaseFilter baseGrabFilter = sampGrabber as IBaseFilter;
                ConfigureSampleGrabber(sampGrabber);

                // Add the frame grabber to the graph
                status = _filterGraph.AddFilter(baseGrabFilter, ".net grabber");
                DsError.ThrowExceptionForHR(status);

                //TODO: Set Framerate, height and width here

                status = capGraph.RenderStream(PinCategory.Capture, MediaType.Video, capFilter, null, baseGrabFilter);
                DsError.ThrowExceptionForHR(status);

                SaveSizeInfo(sampGrabber);

                _bitmapMemory = Marshal.AllocCoTaskMem(_stride * _videoHeight);
            }
            finally
            {
                if (capFilter != null)
                {
                    Marshal.ReleaseComObject(capFilter);
                    capFilter = null;
                }
                if (sampGrabber != null)
                {
                    Marshal.ReleaseComObject(sampGrabber);
                    sampGrabber = null;
                }
                if (capGraph != null)
                {
                    Marshal.ReleaseComObject(capGraph);
                    capGraph = null;
                }
            }
        }

        /// <summary>
        /// Starts the capture graph
        /// </summary>
        public void Start()
        {
            if (!_running)
            {
                int status = _mediaCtrl.Run();
                DsError.ThrowExceptionForHR(status);

                _running = true;
            }
        }

        /// <summary>
        /// Pauses the capture grapg
        /// </summary>
        public void Pause()
        {
            if (_running)
            {
                int status = _mediaCtrl.Pause();
                DsError.ThrowExceptionForHR(status);

                _running = false;
            }
        }

        /// <summary>
        /// Save the Size info of the capture device
        /// </summary>
        /// <param name="sampGrabber"></param>
        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            _videoWidth = videoInfoHeader.BmiHeader.Width;
            _videoHeight = videoInfoHeader.BmiHeader.Height;
            _stride = _videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        /// <summary>
        /// Configures the sample grabber
        /// </summary>
        /// <param name="sampGrabber"></param>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            AMMediaType media;
            int status;

            // Set the media type to Video/RBG24
            media = new AMMediaType();
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            status = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(status);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber
            status = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(status);
        }

        #region ISampleGrabberCB Members

        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            Debug.WriteLine("SampleCB called fir unknown reason ;)");
            return 0;
        }

        public int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            if (_capturePicture)
            {
                _capturePicture = false;

                if (bufferLen > _stride * _videoHeight)
                    throw new ArgumentException("Buffer has the wrong size");

                CopyMemory(_bitmapMemory, pBuffer, bufferLen);

                _captureReady.Set();
            }

            return 0;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            int status;

            try
            {
                if (_mediaCtrl != null)
                {
                    // Stop the graph
                    status = _mediaCtrl.Stop();
                    _running = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            if (_filterGraph != null)
            {
                Marshal.ReleaseComObject(_filterGraph);
                _filterGraph = null;
            }
        }

        #endregion
    }
}
