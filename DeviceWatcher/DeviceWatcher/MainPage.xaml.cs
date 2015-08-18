using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;

using Windows.Devices.Enumeration;
using Windows.Devices.Enumeration.Pnp;
using System.Collections.ObjectModel;

namespace DeviceWatcher
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        public class clsDevType
        {
            public string TypeOfDevice { get; set; }


            public clsDevType(string typeOfDevice)
            {
                TypeOfDevice = typeOfDevice;
            }
        }

        public MainPage()
        {

            this.InitializeComponent();
            //DevTypes.Add(new clsDevType("jjkl"));
            //DevTypes.Add(new clsDevType("ajjkl"));
            //DevTypes.Add(new clsDevType("bjjkl"));
            //cbDevType.DataContext = DevTypes;
        }

        Windows.UI.Core.CoreDispatcher dispatcher;
        public static Windows.Devices.Enumeration.DeviceWatcher watcher = null;
        public static int count = 0;
        public static DeviceInformation[] interfaces = new DeviceInformation[1000];
        public static List<DeviceInformation> MyInterfaces = new List<DeviceInformation>();
        public static bool isEnumerationComplete = false;
        public static string StopStatus = null;

        //public static List<clsDevType> DevTypes = new List<clsDevType>();
        public static List<string> DevTypes = new List<string>();
        //public ObservableCollection<clsDevType> DevTypes = new ObservableCollection<clsDevType>();

        async void WatchDevices_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (watcher != null)
            {
                watcher = null;
            }
            count = 0; 
            isEnumerationComplete = false;
            StopStatus = null;
            //DeviceInterfacesOutputList.Items.Clear();

            try
            {
                dispatcher = Window.Current.CoreWindow.Dispatcher;
                watcher = DeviceInformation.CreateWatcher();
                // Add event handlers
                watcher.Added += watcher_Added;
                watcher.Removed += watcher_Removed;
                watcher.Updated += watcher_Updated;
                watcher.EnumerationCompleted += watcher_EnumerationCompleted;
                watcher.Stopped += watcher_Stopped;
                watcher.Start();
                OutputText.Text = "Enumeration started.";

                this.btnWatchDevices.IsEnabled = false;
                this.btnStop.IsEnabled = !(this.btnWatchDevices.IsEnabled);


            }
            catch (ArgumentException)
            {
                //The ArgumentException gets thrown by FindAllAsync when the GUID isn't formatted properly
                //The only reason we're catching it here is because the user is allowed to enter GUIDs without validation
                //In normal usage of the API, this exception handling probably wouldn't be necessary when using known-good GUIDs 
                OutputText.Text = "Caught ArgumentException. Failed to create watcher.";
            }
        }

        async void StopWatcher(object sender, RoutedEventArgs eventArgs)
        {
            try
            {
                if (watcher.Status == Windows.Devices.Enumeration.DeviceWatcherStatus.Stopped)
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        OutputText.Text = "The enumeration is already stopped.";
                    });
                }
                else
                {
                    watcher.Stop();
                }
            }
            catch (ArgumentException)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    OutputText.Text = "Caught ArgumentException. Failed to stop watcher.";
                });
            }
            this.btnWatchDevices.IsEnabled = true;
            this.btnStop.IsEnabled = !(this.btnWatchDevices.IsEnabled);
        }

        async void watcher_Added(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformation deviceInterface)
        {
            interfaces[count] = deviceInterface;
            count += 1;
            if (isEnumerationComplete)
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DisplayDeviceInterfaceArray();
                    OutputText.Text = "Watcher added: Enumeration was already complete.";
                });
            }
        }

        async void watcher_Updated(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformationUpdate devUpdate)
        {
            int count2 = 0;

            foreach (DeviceInformation deviceInterface in interfaces)
            {
                if (count2 < count)
                {
                    if (interfaces[count2].Id == devUpdate.Id)
                    {
                        //Update the element.
                        interfaces[count2].Update(devUpdate);
                    }

                }
                count2 += 1;
            }
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = "Enumeration updated: Device added ";
                DisplayDeviceInterfaceArray();
            });
        }

        async void watcher_Removed(Windows.Devices.Enumeration.DeviceWatcher sender, DeviceInformationUpdate devUpdate)
        {;
            int count2 = 0;
            //Convert interfaces array to a list (IList).
            List<DeviceInformation> interfaceList = new List<DeviceInformation>(interfaces);
            foreach (DeviceInformation deviceInterface in interfaces)
            {
                if (count2 < count)
                {
                    if (interfaces[count2].Id == devUpdate.Id)
                    {
                        //Remove the element.
                        interfaceList.RemoveAt(count2);
                    }

                }
                count2 += 1;
            }
            //Convert the list back to the interfaces array.
            interfaces = interfaceList.ToArray();
            count -= 1;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = "Enumeration device was removed. ";
                DisplayDeviceInterfaceArray();
            });
        }

        async void watcher_EnumerationCompleted(Windows.Devices.Enumeration.DeviceWatcher sender, object args)
        {
            isEnumerationComplete = true;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                OutputText.Text = "Enumeration complete. ";
                DisplayDeviceInterfaceArray();
                tblLatest.Text = "";
            });
        }

        async void watcher_Stopped(Windows.Devices.Enumeration.DeviceWatcher sender, object args)
        {
            if (watcher.Status == Windows.Devices.Enumeration.DeviceWatcherStatus.Aborted)
            {
                StopStatus = "Enumeration stopped unexpectedly. Click Watch to restart enumeration.";
            }
            else if (watcher.Status == Windows.Devices.Enumeration.DeviceWatcherStatus.Stopped)
            {
                StopStatus = "You requested to stop the enumeration. Click Watch to restart enumeration.";
            }
        }

        /// <summary>
        /// Turn interfaces into a list and apply filters
        /// Refresh DeviceInterfacesOutputList data
        /// Can sort by name
        /// </summary>
        async void DisplayDeviceInterfaceArray()
        {
            //Get currently selected device type to be compared to first part of Id
            string devType = "";
            if (cbDevType.SelectedIndex != -1)
            {
                devType = (string)cbDevType.SelectedItem;
            }
            //Get devices as list and filter
            var myInterfaces1 = interfaces.ToList<DeviceInformation>();
            var myInterfaces2 = from p in myInterfaces1 where filter(p, devType) select p;

            //Apply set by name if selected
            if (chkSortOnName.IsChecked == true)
            {
                 myInterfaces2 = from p in myInterfaces1 where filter(p,devType) orderby p.Name select p ;
            }

            //Copy to the list used XAML list control and refresh
            MyInterfaces = myInterfaces2.ToList<DeviceInformation>();
            DeviceInterfacesOutputList.DataContext = MyInterfaces;
            tbCount.Text = MyInterfaces.Count.ToString();

            //Update list of devTypes
            var dt = from d in myInterfaces1 where d != null select getDevType(d.Id);
            //getDevType() can return nulls for if id name is a Guid
            var dt2 = from d2 in dt where d2 != null select d2;
            var dt3 = (from d3 in dt2 select d3.TypeOfDevice).Distinct<string>();
            DevTypes = dt3.ToList<string>();
            DevTypes= DevTypes.OrderBy(q => q).ToList();
            cbDevType.DataContext = DevTypes;

            //Used when XAML list was created manually:
            //DeviceInterfacesOutputList.Items.Clear();
            //int count2 = 0;
            //foreach (DeviceInformation deviceInterface in interfaces)
            //{
            //    if (count2 < count)
            //    {
            //        DisplayDeviceInterface(deviceInterface);
            //    }
            //    count2 += 1;
            //}
        }

        clsDevType getDevType(string id)
        {
            id = id.Substring(0, 12);
            string[] items = id.Split(new char[] { '#' });
            string item = items[0].Replace("\\\\?\\", "");
            if (item.Contains("{"))
                return null;
            return new clsDevType(items[0].Replace("\\\\?\\", ""));
        }

        /// <summary>
        /// Apply filters for Linq query
        /// </summary>
        /// <param name="deviceInterface"></param>
        /// <returns>true if interface passes filter tests</returns>
        bool filter(DeviceInformation deviceInterface, string devType)
        {
            if (deviceInterface == null)
                return false;

            if ((!deviceInterface.IsEnabled) && (!ShowHidden.IsChecked == true))
                return false;

            if (tbIgnore.Text != "")
            {
                if (!(deviceInterface.Name.Length < tbIgnore.Text.Length))
                    if (deviceInterface.Name.Substring(0, tbIgnore.Text.Length) == tbIgnore.Text)
                        return false;
            }
            if (OnlyUSB.IsChecked == true)
            {
                if (!deviceInterface.Name.ToUpper().Contains("USB"))
                    return false;
            }
            else
            {
                if (devType != "")
                {
                    if (cbSelect.IsChecked == true)
                    { 
                        string[] items = deviceInterface.Id.Split(new char[] { '#' });
                        string _devType = items[0].Replace("\\\\?\\", "");
                        if (_devType != devType)
                            return false;
                    }
                }
            }
            if (tbFilter.Text != "")
            {
                if (!deviceInterface.Name.ToUpper().Contains(tbFilter.Text.ToUpper()))
                    return false;
            }
            if (tbIDIncludes.Text != "")
            {
                if (!deviceInterface.Id.ToUpper().Contains(tbIDIncludes.Text.ToUpper()))
                    return false;
            }
            return true;
        }

        //The following was used when list was created manually from interfaces:
        //void DisplayDeviceInterface(DeviceInformation deviceInterface)
        //{
        //    if ((!deviceInterface.IsEnabled) && (!ShowHidden.IsChecked == true))
        //        return;
        //    if (tbIgnore.Text != "")
        //    {
        //        if (!(deviceInterface.Name.Length < tbIgnore.Text.Length))
        //            if (deviceInterface.Name.Substring(0, tbIgnore.Text.Length) == tbIgnore.Text)
        //                return;
        //    }
        //    if (OnlyUSB.IsChecked == true)
        //    {
        //        if (!deviceInterface.Name.ToUpper().Contains("USB"))
        //            return;
        //    }
        //    if (tbFilter.Text != "")
        //    {
        //        if (!deviceInterface.Name.ToUpper().Contains(tbFilter.Text.ToUpper()))
        //            return;
        //    }
        //    if (tbIDIncludes.Text != "")
        //    {
        //        if (!deviceInterface.Id.ToUpper().Contains(tbIDIncludes.Text.ToUpper()))
        //            return;
        //    }
        //    https://msdn.microsoft.com/en-us/library/windows.devices.enumeration.deviceinformationkind.aspx
        //    string kind = "";
        //    switch ((int)deviceInterface.Kind)
        //    {
        //        case 0:
        //            kind = "Unknown";
        //            break;
        //        case 1:
        //            kind = "DeviceInterface";
        //            break;
        //        case 2:
        //            kind = "DeviceContainer ";
        //            break;
        //        case 3:
        //            kind = "Device\\Devnode";
        //            break;
        //        case 4:
        //            kind = "DeviceInterfaceClass";
        //            break;
        //        case 5:
        //            kind = "AssociationEndpoint";
        //            break;
        //        case 6:
        //            kind = "AssociationEndpointContainer";
        //            break;
        //        case 7:
        //            kind = "AssociationEndpointService";
        //            break;
        //    }
        //    var id = "Id:" + deviceInterface.Id;
        //    var name = deviceInterface.Name;
        //    var isEnabled = "IsEnabled:" + deviceInterface.IsEnabled;


        //    var item = id + " is \n" + name + "\nKind = " + kind + " | " + isEnabled;


        //    DeviceInterfacesOutputList.Items.Add(item);

        //}

        /// <summary>
        /// When a list item is selected tejh Id is parsed to extract specific elements of the interface
        /// Could add more.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeviceInterfacesOutputList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DeviceInterfacesOutputList.SelectedIndex == -1)
            {
                tblLatest.Text = "";
                return;
            }
            string[] vidpid = { "", "" };
            DeviceInformation deviceInfo = (DeviceInformation) DeviceInterfacesOutputList.SelectedItem;
            string name = deviceInfo.Name;
            string id = deviceInfo.Id;
            string comport = "";
            if (name.Contains("USB Serial Device (COM"))
            {
                string[] comports = name.Split(new char[] { '(', ')' });
                name = comports[0];
                comport = comports[1];
            }
            tblLatest.Text = name;
            if (comport != "")
                tblLatest.Text += "     Port: " + comport;
            string[] items = id.Split(new char[] { '#' });
            tblLatest.Text += "    Class:" + items[0].Replace("\\\\?\\","");
            if ((items[1].Contains("VID_")) && (items[1].Contains("PID_")))
            {
                vidpid = items[1].Split(new char[] { '&' });                   
                tblLatest.Text += "\n" + vidpid[0].Replace("_",":");
                tblLatest.Text += "     " + vidpid[1].Replace("_", ":");
                tblLatest.Text += "     ClassID:" + items[2];
                tblLatest.Text += "     GUID:" + items[3];
            }
            else if ((items[1].Contains("FUNC_")) && (items[1].Contains("VEN_")) && (items[1].Contains("DEV_")))
            {
                vidpid = items[1].Split(new char[] { '&' });
                tblLatest.Text += "\n" + vidpid[0].Replace("_", ":");
                tblLatest.Text += "     " + vidpid[1].Replace("_", ":");
                tblLatest.Text += "     " + vidpid[2].Replace("_", ":");
                tblLatest.Text += "     ClassID:" + items[2];
                tblLatest.Text += "     GUID:" + items[3];
            }

            tblLatest.Text +="\n" + id;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //buttonDropDown.IsOpen = true;
        }
    }
}
