using CRADLE_MONITOR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Phone.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.Notification;
using Windows.UI.Popups;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Cradle_Monitor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class enterBabyName : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public enterBabyName()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed_enterBabyName;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("Baby_Name"))
                enterBabyNameTextBox1.Text = localSettings.Values["Baby_Name"].ToString();
            if (localSettings.Values.ContainsKey("Baby_Weight"))
                enterBabyNameTextBox2.Text = localSettings.Values["Baby_Weight"].ToString();
            if (localSettings.Values.ContainsKey("Product_ID"))
                enterBabyNameTextBox3.Text = localSettings.Values["Product_ID"].ToString();
            if (localSettings.Values.ContainsKey("Gender"))
            {
                if (localSettings.Values["Gender"].ToString() == "Male")
                    genderCheckBox1.IsChecked = true;
                else if (localSettings.Values["Gender"].ToString() == "Female")
                    genderCheckBox2.IsChecked = true;
            }
            if (localSettings.Values.ContainsKey("Contact_Number"))
                textBox1.Text = localSettings.Values["Contact_Number"].ToString();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration
        private async void enterBabyNameButton_Click(object sender, RoutedEventArgs e)
        {
            //Store all the baby details to an instance of the BabyNameWeightClass and send it to the next page
            //BabyNameWeight b = new BabyNameWeight()
            //{
            //    name = enterBabyNameTextBox1.Text,
            //    weight = enterBabyNameTextBox2.Text,
            //    pruductID = enterBabyNameTextBox3.Text
            //};
            //if (genderCheckBox1.IsChecked == true)
            //{
            //    b.gender = false;
            //}
            //else if (genderCheckBox2.IsChecked == true)
            //{
            //    b.gender = true;
            //}
            if (enterBabyNameTextBox3.Text == "Enter ID here" || enterBabyNameTextBox3.Text == "")
            {
                MessageDialog msgbox = new MessageDialog("Please enter baby Product ID");
                await msgbox.ShowAsync();
            }
            else if (enterBabyNameTextBox1.Text == "Enter name here" || enterBabyNameTextBox1.Text == "")
            {
                MessageDialog msgbox = new MessageDialog("Please enter baby name");
                await msgbox.ShowAsync();
            }
            else if (enterBabyNameTextBox2.Text == "Enter weight here(in kgs)" || enterBabyNameTextBox2.Text == "")
            {
                MessageDialog msgbox = new MessageDialog("Please enter baby weight");
                await msgbox.ShowAsync();
            }

            else if (genderCheckBox1.IsChecked == false && genderCheckBox2.IsChecked == false)
            {
                MessageDialog msgbox = new MessageDialog("Please select baby's gender");
                await msgbox.ShowAsync();
            }
            else if (textBox1.Text == "Enter your number" || textBox1.Text == "")
            {
                MessageDialog msgbox = new MessageDialog("Please enter valid contact number");
                await msgbox.ShowAsync();
            }
            else
            Frame.Navigate(typeof(Monitor));



        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            //Move to settings page.
            Frame.Navigate(typeof(SettingsPage));
           
        }
        private void HardwareButtons_BackPressed_enterBabyName(object sender, BackPressedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        
        //Methods to delete data when textbox is in focus

        private void enterBabyNameTextBox1_GetFocus(object sender, RoutedEventArgs e)
        {
            enterBabyNameTextBox1.Text = "";
        }
        private void enterBabyNameTextBox2_GetFocus(object sender, RoutedEventArgs e)
        {
            enterBabyNameTextBox2.Text = "";
        }


       

        private void enterBabyNameTextBox3_GetFocus(object sender, RoutedEventArgs e)
        {
            enterBabyNameTextBox3.Text = "";
        }
        #endregion

        private void genderCheckBox1_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void genderCheckBox2_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void enterBabyNameTextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Baby_Name"] = enterBabyNameTextBox1.Text;

        }

        private void enterBabyNameTextBox2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Baby_Weight"] = enterBabyNameTextBox2.Text;
        }

        private void enterBabyNameTextBox3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Product_ID"] = enterBabyNameTextBox3.Text;
        }

        private void genderCheckBox1_Checked(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Gender"] = "Male";
        }

        private void genderCheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Gender"] = "Female";
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["Contact_Number"] = textBox1.Text;
        }
    }
}
