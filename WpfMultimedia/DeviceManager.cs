using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectShowLib;

namespace WpfMultimedia
{
    public static class DeviceManager
    {
        public static DsDevice[] GetDevices(Guid deviceCategory)
        {
            return DsDevice.GetDevicesOfCat(deviceCategory).Where(n => !String.IsNullOrWhiteSpace(n.Name)).ToArray();
        }

        public static DsDevice GetDeviceByName(Guid deviceCategory, string deviceName)
        {
            return GetDevices(deviceCategory).SingleOrDefault(d => d.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase));
        }

        public static DsDevice GetDeviceByPath(Guid deviceCategory, string devicePath)
        {
            return GetDevices(deviceCategory).SingleOrDefault(d => d.DevicePath.Equals(devicePath, StringComparison.OrdinalIgnoreCase));
        }

        public static DsDevice[] GetVideoInputDevices()
        {
            return GetDevices(FilterCategory.VideoInputDevice);
        }
    }
}