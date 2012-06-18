using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DevEck.Devices.Common;

namespace DevEck.Devices.Video
{
    public partial class ImageCaptureControl : UserControl
    {
        private struct MaintainAspectRatioInfo
        {
            public bool Resized;
            public Size LastClientSize;
            public int ImageWidth;
            public int ImageHeight;
        }


        /// <summary>
        /// Device to capture from
        /// </summary>
        private Device _device;

        /// <summary>
        /// Captures images ;)
        /// </summary>
        private ImageCapture _capture;

        /// <summary>
        /// Indicates if capturing is active right now
        /// </summary>
        private volatile bool _started = false;

        /// <summary>
        /// Indicates the whole control is filled or not
        /// </summary>
        private volatile bool _maintainAspectRatio = false;

        /// <summary>
        /// Control is currently capturing an image
        /// </summary>
        private volatile bool _currentlyCaptureingImage = false;

        /// <summary>
        /// Contains some size information
        /// </summary>
        private MaintainAspectRatioInfo _maintainAspectRatioInfo = new MaintainAspectRatioInfo();

        /// <summary>
        /// Is set by Capture() and filled by CaptureThread
        /// </summary>
        private Bitmap _captureImage = null;

        /// <summary>
        /// Set by CaptureThread when picture has been captured
        /// </summary>
        private ManualResetEvent _captureEvent = new ManualResetEvent(false);

        /// <summary>
        /// Indicates the whole control is filled or not
        /// </summary>
        public bool MaintainAspectRatio
        {
            get { return _maintainAspectRatio; }
            set { _maintainAspectRatio = value; }
        }

        /// <summary>
        /// Gets or Sets the current capture device
        /// </summary>
        public Device Device
        {
            get { return _device; }
            set
            {
                if (_capture != null)
                {
                    Stop();
                    _capture.Dispose();
                    _capture = null;
                }

                _device = value;

                if (_device != null)
                {
                    _capture = new ImageCapture(_device);
                    _capture.Initialize();
                }
            }
        }

        public ImageCaptureControl()
        {
            InitializeComponent();

            _maintainAspectRatioInfo.Resized = true;
        }

        /// <summary>
        /// Starts the image capturer
        /// </summary>
        public void Start()
        {
            if (_started == false)
            {
                _started = true;
                _capture.Start();
                ThreadPool.QueueUserWorkItem(new WaitCallback(CaptureThread));
            }
        }

        /// <summary>
        /// Stops the capturer
        /// </summary>
        public void Stop()
        {
            _started = false;
            _capture.Pause();
        }

        /// <summary>
        /// Processe the captured images and paints them on the control
        /// </summary>
        /// <param name="state"></param>
        private void CaptureThread(object state)
        {
            while (_started)
            {

                using (ComBitmap currentImage = _capture.Image)
                {
                    if (_currentlyCaptureingImage)
                    {
                        _captureImage = new Bitmap(_capture.OriginalWidth, _capture.OriginalHeight);
                        Graphics captureG = Graphics.FromImage(_captureImage);
                        captureG.DrawImage(currentImage.Bitmap,
                            new Rectangle(0, 0, _captureImage.Width, _captureImage.Height),
                            new Rectangle(0, 0, currentImage.Bitmap.Width, currentImage.Bitmap.Height), GraphicsUnit.Pixel);
                        _captureEvent.Set();
                    }

                    if (_currentlyCaptureingImage == false)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            Graphics g = this.CreateGraphics();

                            if (_maintainAspectRatio)
                            {
                                if (_maintainAspectRatioInfo.Resized)
                                {
                                    RecalculateDrawSize();
                                    _maintainAspectRatioInfo.Resized = false;
                                    g.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
                                }
                                g.DrawImage(currentImage.Bitmap,
                                    new Rectangle(
                                        (int)(((double)(this.ClientSize.Width - _maintainAspectRatioInfo.ImageWidth)) / 2),
                                        (int)(((double)(this.ClientSize.Height - _maintainAspectRatioInfo.ImageHeight)) / 2),
                                        _maintainAspectRatioInfo.ImageWidth,
                                        _maintainAspectRatioInfo.ImageHeight),
                                    new Rectangle(0, 0, currentImage.Bitmap.Width, currentImage.Bitmap.Height),
                                    GraphicsUnit.Pixel);

                            }
                            else
                                g.DrawImage(currentImage.Bitmap, this.ClientRectangle,
                                    new Rectangle(0, 0, currentImage.Bitmap.Width, currentImage.Bitmap.Height), GraphicsUnit.Pixel);
                        });
                    }

                }
            }

        }


        /// <summary>
        /// Captures the current image
        /// </summary>
        /// <returns></returns>
        public new Bitmap Capture()
        {
            _currentlyCaptureingImage = true;
            _captureEvent.WaitOne();
            _captureEvent.Reset();
            Bitmap captureImage = _captureImage;
            _captureImage = null;
            _currentlyCaptureingImage = false;
            return captureImage;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (_maintainAspectRatio && this.ClientSize != _maintainAspectRatioInfo.LastClientSize)
            {
                _maintainAspectRatioInfo.LastClientSize = this.ClientSize;
                _maintainAspectRatioInfo.Resized = true;
            }
        }

        private void RecalculateDrawSize()
        {
            double clientSizeFactor = (double)ClientSize.Width / (double)ClientSize.Height;
            double imageFactor = (double)_capture.OriginalWidth / (double)_capture.OriginalHeight;


            if (clientSizeFactor > imageFactor)
            {
                //Maximize height
                _maintainAspectRatioInfo.ImageHeight = ClientSize.Height;
                _maintainAspectRatioInfo.ImageWidth = (int)((double)ClientSize.Height * imageFactor);
            }
            else
            {
                //Maximize width
                _maintainAspectRatioInfo.ImageWidth = ClientSize.Width;
                _maintainAspectRatioInfo.ImageHeight = (int)((double)ClientSize.Width / imageFactor);
            }
        }
    }
}
