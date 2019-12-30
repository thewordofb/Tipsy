using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tipsy.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritesPage : Page
    {
        public ObservableCollection<Drink> Drinks
        {
            get { return (ObservableCollection<Drink>)GetValue(DrinksProperty); }
            set { SetValue(DrinksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Drinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinksProperty =
            DependencyProperty.Register("Drinks", typeof(ObservableCollection<Drink>), typeof(ResultsPage), new PropertyMetadata(new ObservableCollection<Drink>()));

        public FavoritesPage()
        {

            SetUpPageAnimation();
            this.InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Drinks = new ObservableCollection<Drink>();
            List<Drink> dl = DrinkManager.FavoritesList;
            if (dl != null)
            {
                foreach(Drink d in dl)
                {
                    Drinks.Add(d);
                }
            }

            DrinkList.Drinks = Drinks;
        }

        private void SetUpPageAnimation()
        {
            TransitionCollection collection = new TransitionCollection();
            NavigationThemeTransition theme = new NavigationThemeTransition();

            var info = new ContinuumNavigationTransitionInfo();

            theme.DefaultNavigationTransitionInfo = info;
            collection.Add(theme);
            this.Transitions = collection;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void SplitViewButton_Click(object sender, RoutedEventArgs e)
        {
            ResultSplitView.IsPaneOpen = !ResultSplitView.IsPaneOpen;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CreateDrinkPage));
        }
    }
}
