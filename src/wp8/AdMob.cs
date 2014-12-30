using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Windows;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;
using GoogleAds;

using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;

namespace Cordova.Extension.Commands
{
    public class AdMob : BaseCommand
    {
        private AdView bannerAd;
        private InterstitialAd interstitialAd;
        private AdRequest adRequest;

        private AdFormats getAdSize(String size)
        {
            if ("BANNER".Equals(size))
            {
                return AdFormats.Banner;
            }
            return AdFormats.SmartBanner;
        }

        public void createInterstitialView(string args)
        {
            interstitialAd = new InterstitialAd("ca-app-pub-5679180806221655/5133703081");
            AdRequest adRequest = new AdRequest();
            // Enable test ads.
            //adRequest.ForceTesting = true;

            interstitialAd.ReceivedAd += OnInterticialAdReceived;
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                interstitialAd.LoadAd(adRequest);
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            });

        }

        private void OnInterticialAdReceived(object sender, AdEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Ad received successfully");
            interstitialAd.ShowAd();
        }

        public void setOptions(string args)
        {
           /* Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GoogleAnalytics.EasyTracker.GetTracker().SendView("MainPage");
            });*/
        }

        public void showAd(string args)
        {
            string[] inputs;
            try
            {
                inputs = JsonHelper.Deserialize<string[]>(args);
            }
            catch
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
                return;
            }
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                bannerAd.LoadAd(adRequest);
                this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
            });
        }

        public void requestAd(string args)
        {
            string[] inputs;
            try
            {
                inputs = JsonHelper.Deserialize<string[]>(args);
            }
            catch
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
                return;
            }
            adRequest = new AdRequest();
            adRequest.ForceTesting = Convert.ToBoolean(inputs[0]);

            this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
        }

        public void createBannerView(string args)
        {
            string[] inputs;

            try
            {
                inputs = JsonHelper.Deserialize<string[]>(args);

                var publisherId = "ca-app-pub-5679180806221655/3656969887";
                var size = "30";
                var bannerAtTop = "false";

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (frame != null)
                    {
                        PhoneApplicationPage page = frame.Content as PhoneApplicationPage;

                        if (page != null)
                        {
                            Grid grid = page.FindName("LayoutRoot") as Grid;
                            if (grid != null)
                            {
                                RowDefinition row = new RowDefinition();
                                row.Height = GridLength.Auto;
                                grid.RowDefinitions.Add(row);

                                bannerAd = new AdView
                                {
                                    Format = getAdSize(size),
                                    AdUnitID = publisherId
                                };

                                bannerAd.ReceivedAd += OnAdReceived;
                                bannerAd.FailedToReceiveAd += OnFailedToReceiveAd;

                                grid.Children.Add(bannerAd);
                                Grid.SetRow(bannerAd, 1);
                            }
                        }
                    }

                    this.DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
                });
            }
            catch
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));

                return;
            }
        }
        private void OnAdReceived(object sender, AdEventArgs e)
        {
            //Debug.WriteLine("Received ad successfully");
        }

        private void OnFailedToReceiveAd(object sender, AdErrorEventArgs errorCode)
        {
            //Debug.WriteLine("Failed to receive ad with error " + errorCode.ErrorCode);
        }

    }
}
