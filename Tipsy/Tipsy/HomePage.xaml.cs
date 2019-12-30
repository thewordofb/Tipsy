using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tipsy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
         private StoreContext context = null;

        public HomePage()
        {
            this.InitializeComponent();
            Window.Current.SetTitleBar(TitleBarBlock);

            if (!Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported())
            {
                this.feedbackButton.Visibility = Visibility.Collapsed;
            }

            GetLicenseInfo();

            HamburgerMenu.SelectedIndex = 0;
            ContentFrame.Navigate(typeof(Pages.SearchPage));

            if(DrinkManager.launchCount == 0)
            {
                FeedbackPopup.IsOpen = true;
            }
            if(DrinkManager.launchCount == 1)
            {
                RateAppPopup.IsOpen = true;
            }
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Pages.SearchPage));
        }

        private void IngredientButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Pages.SearchIngredientPage));
        }

        private void FavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Pages.FavoritesPage));
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(Pages.AboutPage));
        }


        private void HamburgerMenu_ItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerMenu.IsPaneOpen = false;
            if (e.ClickedItem == SearchItem)
            {
                SearchButton_Click(sender, e);
            }
            else if (e.ClickedItem == IngredientsItem)
            {
                IngredientButton_Click(sender, e);
            }
            else if (e.ClickedItem == FavoritesItem)
            {
                FavoritesButton_Click(sender, e);
            }
        }

        private void HamburgerMenu_OptionsItemClick(object sender, ItemClickEventArgs e)
        {
            HamburgerMenu.IsPaneOpen = false;
            AboutButton_Click(sender, e);
        }

        private void AdFreeHyperlink_Click(object sender, RoutedEventArgs e)
        {
            PurchaseAddOn("9npzcrvt78zn");
        }

        public async void GetLicenseInfo()
        {
            if (context == null)
            {
                context = StoreContext.GetDefault();
            }

            StoreAppLicense appLicense = await context.GetAppLicenseAsync();

            if (appLicense == null)
            {
                return;
            }

            foreach (KeyValuePair<string, StoreLicense> item in appLicense.AddOnLicenses)
            {
                StoreLicense addOnLicense = item.Value;
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
            }

            StorePurchaseResult result = await context.RequestPurchaseAsync(storeId);

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
                    DrinkManager.IsAdFree = true;
                    break;

                case StorePurchaseStatus.Succeeded:
                    AdGrid.Visibility = Visibility.Collapsed;
                    DrinkManager.IsAdFree = true;
                    break;

                case StorePurchaseStatus.NotPurchased:
                    break;

                case StorePurchaseStatus.NetworkError:
                    break;

                case StorePurchaseStatus.ServerError:
                    break;
                default:
                    break;
            }
        }

    }
}
