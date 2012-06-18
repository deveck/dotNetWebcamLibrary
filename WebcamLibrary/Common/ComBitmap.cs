/* 
 * @author: deveck.net
 * 
 * This software is protected by Ms-PL 
 * see http://www.microsoft.com/opensource/licenses.mspx#Ms-PL or license.txt
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DevEck.Devices.Common
{
    /// <summary>
    /// Creates a managed Bitmap from pointer to bitmap data
    /// </summary>
    public class ComBitmap:IDisposable
    {
        /// <summary>
        /// Pointer to the bitmap data
        /// Needs to be freed
        /// </summary>
        private IntPtr _bitmapData;

        /// <summary>
        /// Managed Bitmap
        /// </summary>
        private Bitmap _myBitmap;

        /// <summary>
        /// Returns the managed Bitmap object
        /// </summary>
        public Bitmap Bitmap
        {
            get { return _myBitmap; }
        }

        internal ComBitmap(IntPtr bitmapData, int width, int height, int stride, PixelFormat format)
        {
            _bitmapData = bitmapData;

            _myBitmap = new Bitmap(width, height, stride, format, bitmapData);
            _myBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_bitmapData != IntPtr.Zero)
            {
                _myBitmap.Dispose();
                Marshal.FreeCoTaskMem(_bitmapData);
                _bitmapData = IntPtr.Zero;
            }
        }

        #endregion
    }
}
