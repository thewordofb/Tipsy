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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tipsy.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            SetUpPageAnimation();
            this.InitializeComponent();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string[] delim = { "," };
            string[] ingredients = IngredientList.Text.Split(delim, StringSplitOptions.RemoveEmptyEntries);

            List<Drink> results = DrinkManager.Search(DrinkName.Text, ingredients.ToList(), AlcoholStrings(), CategoryStrings());

            Frame.Navigate(typeof(ResultsPage), results);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            DrinkName.Text = "";
            IngredientList.Text = "";

            AlcoholAlcoholic.IsChecked = true;
            AlcoholNonAlcoholic.IsChecked = true;
            AlcoholOptional.IsChecked = true;

            CategoryCocktail.IsChecked = true;
            CategoryCocoa.IsChecked = true;
            CategoryCoffee.IsChecked = true;
            CategoryLiqueur.IsChecked = true;
            CategoryMilk.IsChecked = true;
            CategoryOrdinaryDrink.IsChecked = true;
            CategoryOther.IsChecked = true;
            CategoryPunch.IsChecked = true;
            CategoryShot.IsChecked = true;
            CategorySoftDrink.IsChecked = true;
        }

        private List<string> AlcoholStrings()
        {
            List<string> alcoholTypes = new List<string>();

            if(AlcoholAlcoholic.IsChecked.Value)
            {
                alcoholTypes.Add("Alcoholic");
            }
            if (AlcoholNonAlcoholic.IsChecked.Value)
            {
                alcoholTypes.Add("OnAlcoholic");
            }
            if (AlcoholOptional.IsChecked.Value)
            {
                alcoholTypes.Add("Optional");
            }

            return alcoholTypes;
        }

        private List<string> CategoryStrings()
        {
            List<string> categories = new List<string>();
            //FIX THESE STRINGS
            if (CategoryCocktail.IsChecked.Value) { categories.Add("Cocktails"); }
            if (CategoryCocoa.IsChecked.Value) { categories.Add("Cocoa"); }
            if (CategoryCoffee.IsChecked.Value) { categories.Add("Coffee"); }
            if (CategoryLiqueur.IsChecked.Value) { categories.Add("Liqueur"); }
            if (CategoryMilk.IsChecked.Value) { categories.Add("Milk"); }
            if (CategoryOrdinaryDrink.IsChecked.Value) { categories.Add("Ordinary Drink"); }
            if (CategoryOther.IsChecked.Value) { categories.Add("Other"); }
            if (CategoryPunch.IsChecked.Value) { categories.Add("Punch"); }
            if (CategoryShot.IsChecked.Value) { categories.Add("Shot"); }
            if (CategorySoftDrink.IsChecked.Value) { categories.Add("Soft Drink"); }

            return categories;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length >= 0)
                {
                    var query = from d in DrinkManager.DrinkList
                                where d.Name.ToUpper().Contains(sender.Text.ToUpper())
                                select d.Name;
                    sender.ItemsSource = query.ToList();
                }
                else
                {
                    //Set the ItemsSource to be your filtered dataset
                    //sender.ItemsSource = dataset;
                    sender.ItemsSource = new List<string> { "No results" };
                }
            }
        }


        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            if (args.SelectedItem != null)
            {
                if ((string)args.SelectedItem != "No results")
                { 
                    sender.Text = args.SelectedItem as string;
                }
                else
                {
                    sender.Text = "";
                }
            }
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {
                // Use args.QueryText to determine what to do.
            }
        }

        private void IngredientList_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length >= 0)
                {
                    var query = from d in DrinkManager.IngredientList
                                where d.Name.ToUpper().Contains(sender.Text.ToUpper())
                                select d.Name;
                    sender.ItemsSource = query.ToList();
                }
                else
                {
                    //Set the ItemsSource to be your filtered dataset
                    //sender.ItemsSource = dataset;
                    sender.ItemsSource = new List<string> { "No results" };

                }
            }
        }

        private void IngredientList_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
            }
            else
            {

            }
        }

        private void IngredientList_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            if (args.SelectedItem != null)
            {
                if ((string)args.SelectedItem != "No results")
                {
                    sender.Text = args.SelectedItem as string;
                }
                else
                {
                    sender.Text = "";
                }
            }
        }
    }
}
