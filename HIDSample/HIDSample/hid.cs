using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Devices.Usb;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace HIDSample
{

    public sealed partial class MainPage : Page
    {
        private class Enumeration
        {
            private TextBlock notifyUser;

            public Enumeration(TextBlock _notifyUser)
            {
                notifyUser = _notifyUser;
            }

            private void NotifyUser(string message)
            {
                notifyUser.Text = message;
            }


            /// <summary>
            /// Enumerate HID devices.
            /// Would normally only enumerate one or none devices
            /// </summary>
            /// <param name="vendorId">Vendor Id</param>
            /// <param name="productId">Product Id</param>
            /// <param name="usagePageId">Usage Page Id</param>
            /// <param name="usageId">Usage Id</param>
            public async void EnumerateHidDevices(
                ushort vendorId,
                ushort productId,
                ushort usagePageId,
                ushort usageId
                )
            {
                // Create a selector that gets a HID device using VID/PID and a 
                // VendorDefined usage.
                string selector = HidDevice.GetDeviceSelector(usagePageId, usageId,
                                  vendorId, productId);

                // Enumerate devices using the selector.
                var devices = await DeviceInformation.FindAllAsync(selector);

                if (devices.Count > 0)
                {
                    // Open the target HID device at index 0.
                    HidDevice device = await HidDevice.FromIdAsync(devices.ElementAt(0).Id,
                                       FileAccessMode.ReadWrite);

                    // At this point the device is available to communicate with,
                    // so we can send/receive HID reports from it or 
                    // query it for control descriptions.
                    this.NotifyUser("HID device WAS found");
                }
                else
                {
                    // There were no HID devices that met the selector criteria.
                    this.NotifyUser("HID device NOT found");
                }
            }

            //Having got access to Hid device can access it:
            //HidDevice hidDevice;
            //private async Task GetNumericInputReportAsync()
            //{
            //    var inputReport = await DeviceList.Current.CurrentDevice.GetInputReportAsync(hidDevice.ReadWriteBuffer.ReportId);
            //    var inputReportControl = inputReport.GetNumericControl(hidDevice..ReadWriteBuffer.NumericUsagePage, hidDevice.ReadWriteBuffer.NumericUsageId);
            //    var data = inputReportControl.Value;
            //    this.NotifyUser("Value read: " + data.ToString("X2", NumberFormatInfo.InvariantInfo), NotifyType.StatusMessage);
            //}

            ////////////////////////////////////////////////////////////////////////////////
            /// These direct USB calls don't work??? :

            /// <summary>
            /// Use VID PID and class GUID
            /// </summary>
            /// <param name="deviceVid"></param>
            /// <param name="devicePid"></param>
            /// <param name="deviceInterfaceClassGuid"></param>
            /// <returns></returns>
            public async Task SearchForUSBDevice(ushort deviceVid, ushort devicePid, Guid deviceInterfaceClassGuid)
            {

                string aqs = UsbDevice.GetDeviceSelector(deviceVid, devicePid, deviceInterfaceClassGuid);
                var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs, null);

                if (myDevices.Count == 0)
                {
                    NotifyUser("USB Device not found!");
                    return;
                }

                UsbDevice device = null;
                foreach (var mydevice in myDevices)
                {
                    device = await UsbDevice.FromIdAsync(mydevice.Id);
                    if (device != null)
                        break;
                }
                if (device != null)
                    NotifyUser("USB Device found.");
                else
                    NotifyUser("USB Device not found!");
            }

            /// <summary>
            /// Use VID and PID only
            /// </summary>
            /// <param name="deviceVid"></param>
            /// <param name="devicePid"></param>
            /// <returns></returns>
            public async Task SearchForUSBDevice_VidPid(ushort deviceVid, ushort devicePid)
            {
                string aqs = UsbDevice.GetDeviceSelector(deviceVid, devicePid);
                var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs, null);

                if (myDevices.Count == 0)
                {
                    NotifyUser("USB Device not found!");
                    return;
                }

                UsbDevice device = null;
                foreach (var mydevice in myDevices)
                {
                    device = await UsbDevice.FromIdAsync(mydevice.Id);
                    if (device != null)
                        break;
                }
                if (device != null)
                    NotifyUser("USB Device found.");
                else
                    NotifyUser("USB Device not found!");
            }

            /// <summary>
            /// Use Class GUID only
            /// </summary>
            /// <param name="deviceInterfaceClass"></param>
            /// <returns></returns>
            public async Task SearchForUSBDeviceGuid(Guid deviceInterfaceClass)
            {

                string aqs = UsbDevice.GetDeviceSelector(deviceInterfaceClass);
                var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(aqs, null);
                if (myDevices.Count == 0)
                {
                    NotifyUser("USB Device not found!");
                    return;
                }

                UsbDevice device = null;
                foreach (var mydevice in myDevices)
                {
                    device = await UsbDevice.FromIdAsync(mydevice.Id);
                    if (device != null)
                        break;
                }
                if (device != null)
                    NotifyUser("USB Device found.");
                else
                    NotifyUser("USB Device not found!");
            }


            //Yet another way: UsbDevice.GetDeviceClassSelector
            //byte deviceClass = 0x03;
            //byte deviceSubclass = 0x00;
            //byte protocolCode = 0x01;
            //var myDevices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(
            //                      UsbDevice.GetDeviceClassSelector(
            //                      new UsbDeviceClass()
            //                      {
            //                          ClassCode = deviceClass,
            //                          SubclassCode = deviceSubclass,
            //                          ProtocolCode = protocolCode
            //                      }));

            //SerialDevice.GetDeviceSelectorFromUsbVidPid(venid, devPid);  Does work
        }
    }
    



}
