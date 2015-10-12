using CRADLE_MONITOR.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using Windows.Phone.Notification;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

namespace Cradle_Monitor
{

    public sealed partial class App : Application
    {
        // http://go.microsoft.com/fwlink/?LinkId=290986&clcid=0x409
        public static Microsoft.WindowsAzure.MobileServices.MobileServiceClient cradlemonitorClient = new Microsoft.WindowsAzure.MobileServices.MobileServiceClient(
        "https://cradlemonitor.azure-mobile.net/",
        "FuaytMPbCeJTRUlrbedopCaPFxnJgt63");
        
        private TransitionCollection transitions;

        
        public App()
        {
            this.InitializeComponent();
            
            this.Suspending += this.OnSuspending;
        }

        
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                SuspensionManager.RegisterFrame(rootFrame, "appFrame");

               
                rootFrame.CacheSize = 1;

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    
                    await SuspensionManager.RestoreAsync();
                }

                
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();
        }

        
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync(); 
            
            deferral.Complete();
        }
       
        
    }
    }
