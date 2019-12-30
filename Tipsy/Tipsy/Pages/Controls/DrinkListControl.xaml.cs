using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Tipsy.Pages.Controls
{


    public sealed partial class DrinkListControl : UserControl
    {

        public ObservableCollection<Drink> Drinks
        {
            get { return (ObservableCollection<Drink>)GetValue(DrinksProperty); }
            set { SetValue(DrinksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Drinks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrinksProperty =
            DependencyProperty.Register("Drinks", typeof(ObservableCollection<Drink>), typeof(DrinkListControl), new PropertyMetadata(new ObservableCollection<Drink>(), new PropertyChangedCallback(OnDrinksChanged)));

        private static void OnDrinksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public Drink SelectedDrink
        {
            get { return (Drink)GetValue(SelectedDrinkProperty); }
            set { SetValue(SelectedDrinkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedDrink.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDrinkProperty =
            DependencyProperty.Register("SelectedDrink", typeof(Drink), typeof(DrinkListControl), new PropertyMetadata(0));


        public DrinkListControl()
        {
            this.InitializeComponent();

            DataContext = this;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDrink = listView.SelectedItem as Drink;
        }
    }
}
