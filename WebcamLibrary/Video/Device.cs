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

namespace DevEck.Devices.Video
{
    /// <summary>
    /// Provides simple Access to Video capturing devices
    /// </summary>
    /// <remarks>
    /// This class is just a wrapper around the DsDevice class from DxShow.Net,
    /// which adds some extra functionality
    /// </remarks>
    public class Device
    {
        /// <summary>
        /// DxShow.Net Device
        /// </summary>
        private DsDevice _dsDevice;

        /// <summary>
        /// Provides internal access to the DxShow Device
        /// </summary>
        internal DsDevice DsDevice
        {
            get { return _dsDevice; }
        }

        /// <summary>
        /// Unique Identifier of this device
        /// </summary>
        public string DevicePath
        {
            get { return _dsDevice.DevicePath; }
        }

        /// <summary>
        /// Human readable name of this device
        /// </summary>
        public string FriendlyName
        {
            get { return _dsDevice.Name; }
        }

        internal Device(DsDevice dsDevice)
        {
            _dsDevice = dsDevice;
        }


        public override string ToString()
        {
            return FriendlyName;
        }

        /// <summary>
        /// Looks for a device by its unique identifier
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Device FindDeviceByPath(string path)
        {
            foreach (Device dev in FindDevices())
            {
                if (dev._dsDevice.DevicePath.Equals(path))
                    return dev;
            }

            return null;
        }

        /// <summary>
        /// Finds all available video capture devices
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Device> FindDevices()
        {
            List<Device> devices = new List<Device>();

            foreach (DsDevice dsDevice in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                devices.Add(new Device(dsDevice));

            return devices;
        }
    }
}
