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
    public class IngredientCategory
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public IngredientCategory(string name)
        {
            Ingredients = new List<Ingredient>();
            Name = name;
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchIngredientPage : Page
    {
        List<Ingredient> Ingredients;
        List<String> Categories;
        List<IngredientCategory> GroupedIngredients;
        List<Ingredient> selectedIngredients = new List<Ingredient>();

        public SearchIngredientPage()
        {
            SetUpPageAnimation();

            Ingredients = new List<Ingredient>(DrinkManager.IngredientList);

            //IngredientListView.ItemsSource = Ingredients;

            GroupedIngredients = new List<IngredientCategory>();

            Categories = new List<string>();
            Categories = (from d in Ingredients
                          select d.Category).Distinct().OrderBy(name => name).ToList<string>();

            foreach(string category in Categories)
            {
                if (category != "Other / Unknown" &&
                    category != "Herb / Spice" &&
                    category != "Garnish" )
                {
                    IngredientCategory ingCat = new IngredientCategory(category);

                    ingCat.Ingredients = (from d in Ingredients
                                          where d.Category == category
                                          select d).ToList<Ingredient>();

                    GroupedIngredients.Add(ingCat);

                    foreach(Ingredient ing in ingCat.Ingredients)
                    {
                        if(DrinkManager.DefaultSelectedIngredients.Contains(ing.Name))
                        {
                            selectedIngredients.Add(ing);
                        }
                    }
                }
            }

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



        private void ClearButton_Click(object sender, RoutedEventArgs e)
        { 
            DrinkManager.DefaultSelectedIngredients.Clear();
            IngredientListView.SelectedItems.Clear();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            List<object> selectedIngredients = IngredientListView.SelectedItems.ToList();
            List<string> stringIngredients = (from i in selectedIngredients
                                              select ((Ingredient)i).Name).ToList<string>();
            DrinkManager.DefaultSelectedIngredients = stringIngredients;
            List<Drink> results = DrinkManager.SearchByAvailableIngredients(stringIngredients);

            Frame.Navigate(typeof(ResultsPage), results);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void IngredientListView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Ingredient i in selectedIngredients)
            {
                IngredientListView.SelectedItems.Add(i);
            }
        }
    }
}
