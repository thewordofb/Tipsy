using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class CreateDrinkPage : Page
    {
        public string DrinkTitle { get; set; }
        public string DrinkInstructions { get; set; }
        public ObservableCollection<Part> DrinkParts { get; set;}

        public CreateDrinkPage()
        {
            SetUpPageAnimation();
            DrinkParts = new ObservableCollection<Part>();
            DrinkParts.Add(new Part() { Amount = string.Empty, IngredientName = string.Empty });

            DrinkTitle = string.Empty;
            DrinkInstructions = string.Empty;

            this.InitializeComponent();

            AlcoholCombo.ItemsSource = DrinkManager.AlcoholTypes;
            CategoryCombo.ItemsSource = DrinkManager.DrinkCategories;

            DataContext = this;
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

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            //There's a bug here, we're not adding the ingredients to the searchable list
            if (AlcoholCombo.SelectedIndex != -1 &&
                CategoryCombo.SelectedIndex != -1 &&
                DrinkTitle.Length > 0 &&
                DrinkInstructions.Length > 0 &&
                DrinkParts.Count > 0)
            {
                Drink d = new Drink();
                d.Alcohol = AlcoholCombo.SelectedItem as string;
                d.Category = CategoryCombo.SelectedItem as string;
                d.Name = DrinkTitle;
                d.Instructions = DrinkInstructions;
                d.Ingredients = new List<Part>(DrinkParts);
                int id = int.Parse(DrinkManager.DrinkList.Last().Id) + 1;
                d.Id = id.ToString();

                DrinkManager.PersonalList.Add(d);
                DrinkManager.DrinkList.Add(d);
                d.ToggleFavorite();

                Frame.GoBack();
            }
            else
            {
                MessageDialog dlg = new MessageDialog("Drink missing required fields", "Incomplete Recipe");
                await dlg.ShowAsync();
            }
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            DrinkParts.Add(new Part() { Amount = string.Empty, IngredientName = string.Empty });
        }

        private void RemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (DrinkParts.Count > 0)
            {
                DrinkParts.RemoveAt(DrinkParts.Count - 1 );
            }
        }
    }
}
