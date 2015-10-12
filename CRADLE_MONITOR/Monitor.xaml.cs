using CRADLE_MONITOR.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using Windows.UI.Popups;
using AForge;
using System.Numerics;

//Class to create variables get details from the database
[DataTable("monitor")]
public class monitor
{
    public string Id { get; set; }

    [JsonProperty(PropertyName = "x")]
    public float x { get; set; }

    [JsonProperty(PropertyName = "y")]
    public float y { get; set; }

    [JsonProperty(PropertyName = "z")]
    public float z { get; set; }

    [JsonProperty(PropertyName = "temp")]
    public float temp { get; set; }

    [JsonProperty(PropertyName = "breathrate")]
    public int breathrate { get; set; }

    [CreatedAt]
    public DateTime CreatedAt { get; set; }

    [UpdatedAt]
    public DateTime UpdatedAt { get; set; }


    [Version]
    public string Version { get; set; }
}


namespace Cradle_Monitor
{

    public sealed partial class Monitor : Page
    {
        // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409
        //Access mobile service.
        public static Microsoft.WindowsAzure.MobileServices.MobileServiceClient cradlemonitorClient = new Microsoft.WindowsAzure.MobileServices.MobileServiceClient(
        "https://cradlemonitor.azure-mobile.net/",
        "FuaytMPbCeJTRUlrbedopCaPFxnJgt63");
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        //Create local mobile service client.
        private IMobileServiceTable<global::monitor> babymonitor = App.cradlemonitorClient.GetTable<global::monitor>();

        //Crate dispatch timer to refresh page.
        DispatcherTimer timer = new DispatcherTimer();
        int count;

        public Monitor()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed_Monitor;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

       
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Go to settings page
            timer.Stop();
            Frame.Navigate(typeof(SettingsPage));

        }
        private void HardwareButtons_BackPressed_Monitor(object sender, BackPressedEventArgs e)
        {
            timer.Stop();
            Frame.Navigate(typeof(MainPage));
        }

        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            //Get data from previous page and set values of elements accordingly.
            this.navigationHelper.OnNavigatedTo(e);
            
            //Make progress ring and run until page has loaded
            MyProgressRing.IsActive = true;

            //check if internet connectivity is available
            bool bb = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();

            if (bb == true)
                refreshrows();
            else
            {
                MessageDialog msgbox = new MessageDialog("No internet connection!");
                await msgbox.ShowAsync();
            }
            Frame.Navigate(typeof(MainPage));
           
        }

       
        private void refreshrows()
        {
            

            //Refresh page every two seconds
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 2);
            bool enabled = timer.IsEnabled;
            timer.Start();

         }

        private async void timer_Tick(object sender, object e)
        {

            //Get data from database
            babymonitor.SystemProperties = MobileServiceSystemProperties.CreatedAt | MobileServiceSystemProperties.UpdatedAt | MobileServiceSystemProperties.Version;
                List<monitor> data = await babymonitor
                     .Where(monitor => monitor.x.ToString() != null && monitor.y.ToString() != null && monitor.z.ToString() != null && monitor.temp.ToString() != null)
                     .OrderByDescending(monitor => monitor.UpdatedAt)
                     .IncludeTotalCount()
                     .ToListAsync();

            //Fill up textboxes
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("Baby_Name")) BabyNameBlock.Text = localSettings.Values["Baby_Name"].ToString();
            monitorTextBlock1.Text = localSettings.Values["Baby_Name"].ToString() + " is ";



            //Call function to calculate room temperature
            //calculateroomtemperature(data);

            //Call function to calculate sleeping position
            calculatesleepingposition(data);

            //Call function to calculate 
            calculatebreathing(data);

           // Call function to calculate baby temperature
             calculatebabytemperature(data);


        }

        //Function to calculate breathing
        private async void calculatebreathing(List<monitor> data)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Complex[] reading = new Complex[120];
            for (int i = 0; i < 120; i++)
            {
                reading[i] = data.ElementAt(i).x + data.ElementAt(i).y + data.ElementAt(i).z;
            }
            
            for (int i=120; i< 128; i++)
            {
                reading[i] = 0;
            }

            double a = Math.Abs(data.ElementAt(0).x - data.ElementAt(1).x);
            double b = Math.Abs(data.ElementAt(0).y - data.ElementAt(1).y);
            double c = Math.Abs(data.ElementAt(0).z - data.ElementAt(1).z);
            string contact;

            if (a > 3 || b > 3 || c > 3)
            {
                breathingHeaderTextBlock.Text = localSettings.Values["Baby_Name"].ToString() + "'s breathing is normal";
                breathingHeaderTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.ForestGreen);
                MyProgressRing.IsActive = false;
            }
            else count++;

            if (count >= 5)
            {
                breathingHeaderTextBlock.Text = localSettings.Values["Baby_Name"].ToString() + "'s breathing is unusual. Please check immediately";
                breathingHeaderTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);

                if (localSettings.Values.ContainsKey("Contact_Number"))
                {
                    contact = localSettings.Values["Contact_Number"].ToString();

                    Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(contact, "Cradle");
                    Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                    msg.Body = "Baby's breathing is unusual!";
                    //msg.Recipients.Add("8056274226");
                    msg.Recipients.Add("10010");
                    await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
                }
                MyProgressRing.IsActive = false;
                if (a > 3 || b > 3 || c > 3) count = 0;
            }


        }


        //Function to calculate sleeping position
        private async void calculatesleepingposition(List<monitor> data)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            float x = data.ElementAt(0).x;
            float y = data.ElementAt(0).y;
            float z = data.ElementAt(0).z;
            double xangle = System.Math.Atan(x / (Math.Sqrt((y * y) + (z * z))));
            double yangle = System.Math.Atan(y / (Math.Sqrt((x * x) + (z * z))));
            double zangle = System.Math.Atan((Math.Sqrt((x * x) + (y * y))) / z);
            xangle *= 180 / 3.141592;
            yangle *= 180 / 3.141592;
            zangle *= 180 / 3.141592;
            

            string a = "his";
            if (localSettings.Values["Gender"].ToString() == "Male") a = "his";
            else if (localSettings.Values["Gender"].ToString() == "Female") a = "her";
            if ((xangle > 30 && xangle < 34) && (yangle > 30 && yangle < 34) && (zangle > 40 && zangle < 52))
            {
                sleepingPositionTextBlock.Text = "Asleep on " + a + " back";
                sleepingPositionTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.ForestGreen);
            }
            else if ((xangle > 35 && xangle < 39) && (yangle > 26 && yangle < 30) && (zangle > 49 && zangle < 53) || ((xangle > 30 && xangle < 35) && (yangle > 37 && yangle < 41) && (zangle > 54 && zangle < 68)))
            {
                sleepingPositionTextBlock.Text = "Asleep on " + a + " side";
                sleepingPositionTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);
            }
            else if ((xangle > 36 && xangle < 40) && (yangle > 34 && yangle < 38) && (zangle > 57 && zangle < 61))
            {

                sleepingPositionTextBlock.Text = "Asleep on " + a + " front";
                sleepingPositionTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);
                if (localSettings.Values.ContainsKey("Contact_Number"))
                {
                    string contact = localSettings.Values["Contact_Number"].ToString();

                    Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(contact, "Cradle");
                    Windows.ApplicationModel.Chat.ChatMessage msg = new Windows.ApplicationModel.Chat.ChatMessage();
                    msg.Body = "Baby's breathing is unusual!";
                    //msg.Recipients.Add("8056274226");

                    await Windows.ApplicationModel.Chat.ChatMessageManager.ShowComposeSmsMessageAsync(msg);
                }
            }
        }

        //private void calculateroomtemperature(List<monitor> data)
        //{
        //    Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            
        //    //throw new NotImplementedException();
        //    float temp1 = data.ElementAt(0).temp;
        //    double temp = (((temp1 * (1200 / 1024)) - 500) / 10) + 25;
        //    temperatureValueTextBlock.Text = temp + " deg celsius";
        //    if (temp >= 16 && temp <= 25)
        //    {
        //        temperatureCommentTextBlock.Text = "Perfect room temperature";
        //        temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.ForestGreen);
        //    }
        //    else if (temp > 25 && temp <= 29)
        //    {
        //        temperatureCommentTextBlock.Text = "Mildly hot";
        //        temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);
        //    }
        //    else if (temp > 29)
        //    {
        //        temperatureCommentTextBlock.Text = "Very hot";
        //        temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);
        //    }
        //    else if (temp >= 12 && temp < 16)
        //    {
        //        temperatureCommentTextBlock.Text = "Mildly cold";
        //        temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);
        //    }
        //    else if (temp < 12)
        //    {
        //        temperatureCommentTextBlock.Text = "Very cold";
        //        temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);
        //    }
        //}


        //Function to calculate temperature of baby
        private void calculatebabytemperature(List<monitor> data)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("Baby_Name")) temperatureTextBlock.Text = localSettings.Values["Baby_Name"].ToString() + "'s temperature :";
            float temp1 = data.ElementAt(0).temp;
            double temp = (((temp1 * (1200 / 1024)) - 500) / 10) + 35;
            temperatureValueTextBlock.Text = temp + " deg celsius";
            if (temp >= 35 && temp <= 39)
            {
                temperatureCommentTextBlock.Text = "Optimal temperature";
                temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.ForestGreen);
            }
            else if (temp > 39 && temp <= 42)
            {
                temperatureCommentTextBlock.Text = "Mild heat";
                temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);
            }
            else if (temp > 42)
            {
                temperatureCommentTextBlock.Text = "Baby may have fever!";
                temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);
            }
            else if (temp >= 30 && temp < 35)
            {
                temperatureCommentTextBlock.Text = "Mild cold";
                temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Orange);
            }
            else if (temp < 35)
            {
                temperatureCommentTextBlock.Text = "Unusually cold";
                temperatureCommentTextBlock.Foreground = new SolidColorBrush(Windows.UI.Colors.Crimson);
            }
            
        }

        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void breathingButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}