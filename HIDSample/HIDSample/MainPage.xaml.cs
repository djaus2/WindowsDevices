using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HIDSample
{

    public sealed partial class MainPage : Page
    {



        private Enumeration HIDEnum;
        public MainPage()
        {
            this.InitializeComponent();

            HIDEnum = new Enumeration(this.NotifyUser);

            GetUagePageInfo();

            ////JObject o1 = JObject.Parse(File.ReadAllText(".\\usagepage.json"));
            //using (StreamReader file = File.OpenText(".\\usagepage.json"))
            //using(JsonTextReader reader = new JsonTextReader(file))
            //{
            //    JObject jObject = (JObject)JToken.ReadFrom(reader);
            //    var hidUsageClasses =
            //        from p in jObject["hidUsageClasses"]
            //        select new UsagePage ((string)p["PageName"],(ushort)p["PageID"] );
            //    UsagePages = hidUsageClasses.ToList<UsagePage>();

            //    var hidDeviceClasses =
            //        from p in jObject["hidDeviceClasses"]
            //        select new HidDeviceClass((string)p["UsageName"], (ushort)p["PageID"], (ushort)p["UsageID"]);
            //    HidDeviceClasses = hidDeviceClasses.ToList<HidDeviceClass>();

            //    cbDevType.DataContext = HidDeviceClasses;
            //}
        }

        private void EnumerateHidDevices()
        {
            ushort vendorId = ushort.Parse(tb_vid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            ushort productId = ushort.Parse(tb_pid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            ushort usagePage = ushort.Parse(tb_usagepageID.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            ushort usageId = ushort.Parse(tb_usageID.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            HIDEnum.EnumerateHidDevices(vendorId, productId, usagePage, usageId);
        }

        /// <summary>
        /// Set the VID PID Usepage and UaseId for MS Arc Mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void btnSetMSArcMouse_Click(object sender, RoutedEventArgs e)
        {
            NotifyUser.Text = "***";
            //HID Mouse Device
            //Details from Device Manager -Hardware IDs:
            //HID\VID_045E&PID_07B1&REV_0674&MI_01&Col01
            //HID_DEVICE_UP:0001_U:0002  1 and 2 
            ushort vendorId = 0x045E; 
            ushort productId = 0x07B1; 
            ushort usagePage = 0x1; 
            ushort usageId = 0x2; 
            tb_vid.Text = "0x" + vendorId.ToString("X"); 
            tb_pid.Text =  "0x" + productId.ToString("X");
            tb_usagepageID.Text =  "0x" + usagePage.ToString("X");
            tb_usageID.Text =  "0x" + usageId.ToString("X"); ;
            tb_guid.Text = "{4d36e96f-e325-11ce-bfc1-08002be10318}";
        }

        /// <summary>
        /// Set the VID PID Useagepage and UseageId for HP Probook 450  G1 Keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetHPPB450G_Keyboard_Click(object sender, RoutedEventArgs e)
        {
            NotifyUser.Text = "***";
            //HID Keyboard Device
            //Details from Device Manager  -Hardware IDs:
            //HID\VID_045E&PID_07B1&REV_0674&MI_00
            //HID\VID_049F&PID_0051&REV_0105&MI_00
            //HID_DEVICE_UP:0001_U:0006  1 and 6
            ushort vendorId = 0x045E;
            ushort productId = 0x07B1; 
            ushort usagePage = 0x1; 
            ushort usageId = 0x6; 
            tb_vid.Text = "0x" + vendorId.ToString("X");
            tb_pid.Text = "0x" + productId.ToString("X");
            tb_usagepageID.Text = "0x" + usagePage.ToString("X");
            tb_usageID.Text = "0x" + usageId.ToString("X");
            tb_guid.Text =  "{4d36e96b-e325-11ce-bfc1-08002be10318}"; 

        }

        /// <summary>
        /// Set the VID PID Useagepage and UseageId for HP Desktop USB Keyboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHPDesktopUSBKeyboard_Click(object sender, RoutedEventArgs e)
        {
            NotifyUser.Text = "***";
            //HID Keyboard Device
            //Details from Device Manager  -Hardware IDs:
            //HID\VID_049F&PID_0051&REV_0105&MI_00
            //HID_DEVICE_UP:0001_U:0006  1 and 6
            ushort vendorId = 0x049F;
            ushort productId = 0x0051;
            ushort usagePage = 0x1;
            ushort usageId = 0x6;
            tb_vid.Text = "0x" + vendorId.ToString("X");
            tb_pid.Text = "0x" + productId.ToString("X");
            tb_usagepageID.Text = "0x" + usagePage.ToString("X");
            tb_usageID.Text = "0x" + usageId.ToString("X");
            tb_guid.Text = "{4d36e96b-e325-11ce-bfc1-08002be10318}";
        }

        private void btnEnumerateHidDevices_Click(object sender, RoutedEventArgs e)
        {
            EnumerateHidDevices();
        }

        private async void btnSearchForUSBDevice_VidPidGuid_Click(object sender, RoutedEventArgs e)
        {
            ushort vendorId = ushort.Parse(tb_vid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            ushort productId = ushort.Parse(tb_pid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            Guid guid = new Guid(tb_guid.Text);

            await HIDEnum.SearchForUSBDevice(vendorId, productId, guid);
        }

        private async void btnSearchForUSBDevice_Guid_Click(object sender, RoutedEventArgs e)
        {
            //tb_guid.Text = "{4d36e967-e325-11ce-bfc1-08002be10318}";
            Guid guid = new Guid(tb_guid.Text);
            await HIDEnum.SearchForUSBDeviceGuid( guid);
        }

        private async void btnSearchForUSBDevice_VidPid_Click(object sender, RoutedEventArgs e)
        {
            ushort vendorId = ushort.Parse(tb_vid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);
            ushort productId = ushort.Parse(tb_pid.Text.Replace("0x", ""), System.Globalization.NumberStyles.AllowHexSpecifier);

            await HIDEnum.SearchForUSBDevice_VidPid(vendorId, productId);
        }

        private void btnSetGenericUSBMouse_Click(object sender, RoutedEventArgs e)
        {
            NotifyUser.Text = "***";
            //HID Mouse Device
            //Details from Device Manager -Hardware IDs:
            //HID\VID_045E&PID_07B1&REV_0674&MI_01&Col01
            //HID_DEVICE_UP:0001_U:0002  1 and 2 
            ushort vendorId = 0x045E;
            ushort productId = 0x07B1;
            ushort usagePage = 0x1;
            ushort usageId = 0x2;
            tb_vid.Text = "0x" + vendorId.ToString("X");
            tb_pid.Text = "0x" + productId.ToString("X");
            tb_usagepageID.Text = "0x" + usagePage.ToString("X");
            tb_usageID.Text = "0x" + usageId.ToString("X"); ;
            tb_guid.Text = "{4d36e96f-e325-11ce-bfc1-08002be10318}";
        }


    }
}
