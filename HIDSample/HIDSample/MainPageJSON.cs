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

namespace HIDSample
{
    /// ////////////////////////////////////////////////////////
    // A USB HID device has a usagepage ID and (sub) usage ID
    // e.g a Mouse is on Page 1 and has a usage of 2
    // To identify a USB HID you also need its VID and PID
    // Ref: http://www.usb.org/developers/hidpage/Hut1_12v2.pdf
    //  Page 1 , Generic Desktop Controls is listed on Page 26
    /// ////////////////////////////////////////////////////////

    /* HID Classes : UsagePages
    01h Generic Desktop Controls 
    02h Simulation Controls
    03h VR Controls
    04h Sport Controls
    05h Game Controls
    06h Generic Device Controls
    07h Keyboard/ Keypad
    08h LEDs
    09h Button
    0Ah Ordinal
    0Bh Telephony Devices
    0Ch Consumer Devices
    0Dh Digitizer
    0Fh Physical Input Device (PID)
    10h Unicode
    14h Alphanumeric Display
    40h Medical Instruments
    80h Monitor Devices
    81h Monitor Enumerated Values
    82h VESA Virtual Controls
    83h VESA Command
    84h Power Device
    85h Battery System
    */

    public sealed partial class MainPage : Page
    {

        private List<UsagePage> UsagePages;

        private class UsagePage
        {
            public UsagePage(string pageName, ushort pageID)
            {
                PageID = pageID;
                PageName = pageName;
            }
            public ushort PageID { get; set; }
            public string PageName { get; set; }
        }

        // This list is used to populate cbDevType dropdown menu
        private List<HidDeviceClass> HidDeviceClasses;
        public class HidDeviceClass
        {
            public HidDeviceClass(string usageName, ushort pageID, ushort usageID)
            {
                UsageName = usageName;
                PageID = pageID;
                UsageID = usageID;
            }

            public string UsageName { get; set; }
            public ushort PageID { get; set; }
            public ushort UsageID { get; set; }
        }

        /// <summary>
        /// Populate the cbDevType dropdown menu by databinding to HidDeviceClasses list
        /// Creates list of Usagepages list as well.
        /// Uses the usagepage.json document that is inclusded in the deployed app.
        /// ... Can be manually edited
        /// </summary>
        private void GetUagePageInfo()
        {

            //JObject o1 = JObject.Parse(File.ReadAllText(".\\usagepage.json"));
            using (StreamReader file = File.OpenText(".\\usagepage.json"))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject jObject = (JObject)JToken.ReadFrom(reader);
                var hidUsageClasses =
                        from p in jObject["hidUsageClasses"]
                        select new UsagePage((string)p["PageName"], (ushort)p["PageID"]);
                UsagePages = hidUsageClasses.ToList<UsagePage>();

                var hidDeviceClasses =
                    from p in jObject["hidDeviceClasses"]
                    select new HidDeviceClass((string)p["UsageName"], (ushort)p["PageID"], (ushort)p["UsageID"]);
                HidDeviceClasses = hidDeviceClasses.ToList<HidDeviceClass>();

                cbDevType.DataContext = HidDeviceClasses;
            }
        }

        /// <summary>
        /// From drop down menu at top right:
        /// Select the HID Device class
        /// Changes the UsagePageID and UsageID textboxes
        /// Note that the specific VID and PID for an HID devices is required as well  for its enumeration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDevType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(cbDevType.SelectedIndex == -1))
            {
                HidDeviceClass seln = (HidDeviceClass)cbDevType.SelectedItem;
                tb_usagepageID.Text = "0x" + seln.PageID.ToString("00");
                tb_usageID.Text = "0x" + seln.UsageID.ToString("00");
            }
        }
    }
}
