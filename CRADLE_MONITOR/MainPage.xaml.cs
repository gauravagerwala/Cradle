using CRADLE_MONITOR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



namespace Cradle_Monitor
{
    
    public sealed partial class MainPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public MainPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed_MainPage;
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
        private async  void mainPageButton_Click(object sender, RoutedEventArgs e)
        {
            
            
            //If first time, move to Enter name page, else to monitor page
            if (NetworkInterface.GetIsNetworkAvailable() == false)
            {
                MessageDialog msgbox = new MessageDialog("Please check your internet connection");
                await msgbox.ShowAsync();
            }

            else
            {
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.ContainsKey("Product_ID"))
                    Frame.Navigate(typeof(Monitor));
                else
                    Frame.Navigate(typeof(enterBabyName));
            }
           
     

        }
        private void HardwareButtons_BackPressed_MainPage(object sender, BackPressedEventArgs e)
        {
            if(Frame.CanGoBack)
            Frame.GoBack(); 
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Go to settings page.
            Frame.Navigate(typeof(SettingsPage));
        }


       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        
        

        #endregion
    }

}
