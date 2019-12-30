using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tipsy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.SetTitleBar(DragPanel);

            if (!Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                this.feedbackButton.Visibility = Visibility.Collapsed;
            }

            //TestLicense();
            GetLicenseInfo();

            ContentFrame.Navigate(typeof(Pages.MainPage));
        }

        private void AdBanner_AdRefreshed(object sender, RoutedEventArgs e)
        {

        }

        private void AdBanner_ErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {

        }

        private async void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }

        private async void feedbackButton_Click(object sender, RoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

        private void AdFreeHyperlink_Click(object sender, RoutedEventArgs e)
        {
            PurchaseAddOn("9npzcrvt78zn");
        }

        private StoreContext context = null;

        public async void TestLicense()
        {
            LicenseInformation li = CurrentApp.LicenseInformation;
            if (li.ProductLicenses["TipsyAdFree"].IsActive)
            {
                // the customer can access this feature
            }
            else
            {
                // the customer can' t access this feature
                string s = await CurrentApp.RequestProductPurchaseAsync("TipsyAdFree", false);
            }
        }

        public async void GetProductInfo()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }

            // Specify the kinds of add-ons to retrieve.
            string[] productKinds = { "Durable" };
            List<String> filterList = new List<string>(productKinds);

            // Specify the Store IDs of the products to retrieve.
            string[] storeIds = new string[] { "9NPZCRVT78ZN", "9p238bk5m48j", "9p238bk5m48j/0010", "9p238bk5m48j/0011" };

           // workingProgressRing.IsActive = true;
            StoreProductQueryResult queryResult =
                await context.GetStoreProductsAsync(filterList, storeIds);
           // workingProgressRing.IsActive = false;

            if (queryResult.ExtendedError != null)
            {
                // The user may be offline or there might be some other server failure.
               // textBlock.Text = $"ExtendedError: {queryResult.ExtendedError.Message}";
                return;
            }

            foreach (KeyValuePair<string, StoreProduct> item in queryResult.Products)
            {
                // Access the Store info for the product.
                StoreProduct product = item.Value;

                // Use members of the product object to access info for the product...
            }
        }

        public async void GetLicenseInfo()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }
            /*
            StoreProductResult queryResult = await context.GetStoreProductForCurrentAppAsync();

            if (queryResult.Product != null)
            {
                // The Store catalog returned an unexpected result.
                //textBlock.Text = "Something went wrong, and the product was not returned.";
                StoreProduct p = queryResult.Product;
                string desc = p.Description;
                foreach(StoreSku sku in p.Skus)
                {
                    string s3 = sku.StoreId;
                    string s2 = sku.Price.FormattedPrice;
                    string s1 = sku.Title;
                    string s = sku.Description;
                }

                // Show additional error info if it is available.
                if (queryResult.ExtendedError != null)
                {
                    //textBlock.Text += $"\nExtendedError: {queryResult.ExtendedError.Message}";
                }

            }

            GetProductInfo();


            string[] productKinds = { "Durable", "Consumable", "UnmanagedConsumable" };
            List<String> filterList = new List<string>(productKinds);
            StoreProductQueryResult result = await context.GetAssociatedStoreProductsAsync(filterList);
            foreach( KeyValuePair<string, StoreProduct> product in result.Products )
            {
                string text = product.Key;
                StoreProduct prod = product.Value;
                string token = prod.InAppOfferToken;
                string storeid = prod.StoreId;
            }*/

          //  workingProgressRing.IsActive = true;
            StoreAppLicense appLicense = await context.GetAppLicenseAsync();
          //  workingProgressRing.IsActive = false;

            if (appLicense == null)
            {
               // textBlock.Text = "An error occurred while retrieving the license.";
                return;
            }

            // Use members of the appLicense object to access license info...

            // Access the add on licenses for add-ons for this app.
            foreach (KeyValuePair<string, StoreLicense> item in appLicense.AddOnLicenses)
            {
                StoreLicense addOnLicense = item.Value;
                // Use members of the addOnLicense object to access license info
                // for the add-on...
                //addOnLicense.
                if (addOnLicense.IsActive)
                {
                    AdGrid.Visibility = Visibility.Collapsed;
                    DrinkManager.IsAdFree = true;
                }
            }
        }

        public async void PurchaseAddOn(string storeId)
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
                // If your app is a desktop app that uses the Desktop Bridge, you
                // may need additional code to configure the StoreContext object.
                // For more info, see https://aka.ms/storecontext-for-desktop.
            }

           // workingProgressRing.IsActive = true;
            StorePurchaseResult result = await context.RequestPurchaseAsync(storeId);
           // workingProgressRing.IsActive = false;

            // Capture the error message for the operation, if any.
            string extendedError = string.Empty;
            if (result.ExtendedError != null)
            {
                extendedError = result.ExtendedError.Message;
            }

            switch (result.Status)
            {
                case StorePurchaseStatus.AlreadyPurchased:
                    AdGrid.Visibility = Visibility.Collapsed;
                    break;

                case StorePurchaseStatus.Succeeded:
                    AdGrid.Visibility = Visibility.Collapsed;
                    break;

                case StorePurchaseStatus.NotPurchased:
               //     textBlock.Text = "The purchase did not complete. " +
              //          "The user may have cancelled the purchase. ExtendedError: " + extendedError;
                    break;

                case StorePurchaseStatus.NetworkError:
              //      textBlock.Text = "The purchase was unsuccessful due to a network error. " +
              //          "ExtendedError: " + extendedError;
                    break;

                case StorePurchaseStatus.ServerError:
              //      textBlock.Text = "The purchase was unsuccessful due to a server error. " +
              //          "ExtendedError: " + extendedError;
                    break;

                default:
              //      textBlock.Text = "The purchase was unsuccessful due to an unknown error. " +
              //          "ExtendedError: " + extendedError;
                    break;
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if(e.Content is Pages.MainPage)
            {
                MainWrapGrid.Background = ((Pages.MainPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.AboutPage)
            {
                MainWrapGrid.Background = ((Pages.AboutPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.CreateDrinkPage)
            {
                MainWrapGrid.Background = ((Pages.CreateDrinkPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.FavoritesPage)
            {
                MainWrapGrid.Background = ((Pages.FavoritesPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.ResultsPage)
            {
                MainWrapGrid.Background = ((Pages.ResultsPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.SearchPage)
            {
                MainWrapGrid.Background = ((Pages.SearchPage)e.Content).MainGrid.Background;
            }
            else if(e.Content is Pages.SearchIngredientPage)
            {
                MainWrapGrid.Background = ((Pages.SearchIngredientPage)e.Content).MainGrid.Background;
            }
        }
    }
}
