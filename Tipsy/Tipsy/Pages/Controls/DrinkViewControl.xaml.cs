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
using Microsoft.Advertising.WinRT.UI;
using Windows.Graphics.Printing;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Tipsy.Pages.Controls
{
    public sealed partial class DrinkViewControl : UserControl
    {
        InterstitialAd myInterstitialAd = null;
        string myAppId = "f20092ce-4ccd-4c51-accf-9c46456ec956";
        string myAdUnitId = "11688078";
        //string myAdUnitId = "11673098";
        private PrintHelper printHelper;

        private DataTransferManager dataTransferManager;

        public Drink Drink
        {
            get { return (Drink)GetValue(DrinkProperty); }
            set { SetValue(DrinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Drink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinkProperty =
            DependencyProperty.Register("Drink", typeof(Drink), typeof(DrinkViewControl), new PropertyMetadata(0, new PropertyChangedCallback(OnDrinkChanged)));

        private static void OnDrinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DrinkManager.ShouldDisplayInterstitial())
            {
                if (InterstitialAdState.Ready == ((DrinkViewControl)d).myInterstitialAd.State)
                {
                    ((DrinkViewControl)d).myInterstitialAd.Show();
                }
                else
                {
                    DrinkManager.DidDisplayAd(false);
                }
            }

            if (e.NewValue == null)
            {
                ((DrinkViewControl)d).DefaultView.Visibility = Visibility.Visible;
                ((DrinkViewControl)d).DrinkView.Visibility = Visibility.Collapsed;
            }
            if (e.NewValue != null)
            {
                ((DrinkViewControl)d).DefaultView.Visibility = Visibility.Collapsed;
                ((DrinkViewControl)d).DrinkView.Visibility = Visibility.Visible;

                if (((DrinkViewControl)d).Drink.Favorite)
                {
                    ((DrinkViewControl)d).FavoriteButton.Content = "\uE00B";
                }
                else
                {
                    ((DrinkViewControl)d).FavoriteButton.Content = "\uE006";
                }
            }

            ((DrinkViewControl)d).drinkReset();
        
        }

        public DrinkViewControl()
        {
            myInterstitialAd = new InterstitialAd();
            myInterstitialAd.AdReady += MyInterstitialAd_AdReady;
            myInterstitialAd.ErrorOccurred += MyInterstitialAd_ErrorOccurred;
            myInterstitialAd.Completed += MyInterstitialAd_Completed;
            myInterstitialAd.Cancelled += MyInterstitialAd_Cancelled;
            myInterstitialAd.RequestAd(AdType.Display, myAppId, myAdUnitId);

            this.InitializeComponent();
            DataContext = this;

            if(PrintManager.IsSupported())
            {
                PrintButton.Visibility = Visibility.Visible;
            }
            else
            {
                PrintButton.Visibility = Visibility.Collapsed;
            }

            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
        }

        ~DrinkViewControl()
        {
            if(printHelper != null)
            {
                printHelper.UnregisterForPrinting();
            }

            dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(OnDataRequested);
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequestDeferral deferral = args.Request.GetDeferral();
            DataPackage requestData = new DataPackage();

            requestData.Properties.Title = "Tipsy Recipe";
            requestData.SetHtmlFormat(HTMLHelper.Generate(Drink));

            args.Request.Data = requestData;
            deferral.Complete();

            
        }

        public void drinkReset()
        {
            if(printHelper != null)
            {
                printHelper.UnregisterForPrinting();
                printHelper = null;
            }
        }

        #region Interstitial Ad Events
        void MyInterstitialAd_AdReady(object sender, object e)
        {
            // Your code goes here.
        }

        void MyInterstitialAd_ErrorOccurred(object sender, AdErrorEventArgs e)
        {
            // Your code goes here.
            myInterstitialAd.RequestAd(AdType.Display, myAppId, myAdUnitId);
            DrinkManager.DidDisplayAd(false);
        }

        void MyInterstitialAd_Completed(object sender, object e)
        {
            myInterstitialAd.RequestAd(AdType.Display, myAppId, myAdUnitId);
            DrinkManager.DidDisplayAd(true);
        }

        void MyInterstitialAd_Cancelled(object sender, object e)
        {
            // Your code goes here.
        }
        #endregion

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            Drink.ToggleFavorite();
            if(Drink.Favorite)
            {
                FavoriteButton.Content = "\uE00B";
            }
            else
            {
                FavoriteButton.Content = "\uE006";
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (PrintManager.IsSupported())
            {
                // Initialize print content for this scenario
                if (printHelper == null)
                {
                    printHelper = new PrintHelper(this);
                    printHelper.RegisterForPrinting();
                }
                printHelper.PreparePrintContent(new PageToPrint(Drink));

                await printHelper.ShowPrintUIAsync();

            }
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
    }
}
