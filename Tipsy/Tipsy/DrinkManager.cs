using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.IO;
using System.Reflection;
using Microsoft.Toolkit.Uwp;

namespace Tipsy
{
    public static class DrinkManager
    {

        private static int drinksDisplayed;
        private static bool adDisplayed;
        public static int launchCount;

        public static bool IsAdFree = false;

        public static bool ShouldDisplayInterstitial()
        {
            bool shouldDisplay = false;
            if( !IsAdFree )
            { 
                drinksDisplayed++;

                shouldDisplay = (drinksDisplayed % 5) == 0 || drinksDisplayed == 1;
            }
            return shouldDisplay;
        }

        public static void DidDisplayAd(bool didDisplay)
        {
            adDisplayed = didDisplay;
        }

        public static void Load()
        {
           // IsAdFree = false;
            adDisplayed = true;
            drinksDisplayed = 0;

            var localObjectStorageHelper = new LocalObjectStorageHelper();
            launchCount = localObjectStorageHelper.Read<int>("LaunchCount", 0);
            PersonalList = localObjectStorageHelper.Read<List<Drink>>("PersonalDrinks", new List<Drink>());
            DefaultSelectedIngredients = localObjectStorageHelper.Read<List<string>>("DefaultSelectedIngredients", new List<string>());
            List<Drink> FavoritesDrinks = localObjectStorageHelper.Read<List<Drink>>("FavoriteDrinks", new List<Drink>());

            DrinkList = new List<Drink>();
            IngredientList = new List<Ingredient>();
            PartList = new List<Part>();
            DrinkCategories = new List<string>();
            DrinkCategories.Add("Cocktails"); 
            DrinkCategories.Add("Cocoa"); 
            DrinkCategories.Add("Coffee"); 
            DrinkCategories.Add("Liqueur"); 
            DrinkCategories.Add("Milk"); 
            DrinkCategories.Add("Ordinary Drink"); 
            DrinkCategories.Add("Other"); 
            DrinkCategories.Add("Punch"); 
            DrinkCategories.Add("Shot"); 
            DrinkCategories.Add("Soft Drink"); 

            AlcoholTypes = new List<string>();
            AlcoholTypes.Add("Alcoholic");
            AlcoholTypes.Add("Non-Alcoholic");
            AlcoholTypes.Add("Optional");

            var assembly = typeof(DrinkManager).GetTypeInfo().Assembly;

            var ingredientStream = assembly.GetManifestResourceStream("Tipsy.Assets.ingredients.csv");
            StreamReader ingredientReader = new StreamReader(ingredientStream);
            while(!ingredientReader.EndOfStream)
            {
                string line = ingredientReader.ReadLine();
                string[] token = { "\",\"" };
                line = line.Trim('\"');
                string[] items = line.Split(token, StringSplitOptions.None);

                Ingredient i = new Ingredient() { Id = items[0], Name = items[1], Description = items[2], Category = items[3], Alcohol = items[4] };
                IngredientList.Add(i);
            }

            var drinkStream = assembly.GetManifestResourceStream("Tipsy.Assets.drink.csv");
            StreamReader drinkReader = new StreamReader(drinkStream);
            while (!drinkReader.EndOfStream)
            {
                string line = drinkReader.ReadLine();
                string[] token = { "\",\"" };
                line = line.Trim('\"');
                string[] items = line.Split(token, StringSplitOptions.None);

                Drink d = new Drink() { Id = items[0], Name = items[1], Instructions = items[2], ServeIn = items[3], Category = items[4], Alcohol = items[5] };
                DrinkList.Add(d);
            }

            var partStream = assembly.GetManifestResourceStream("Tipsy.Assets.drinkPart.csv");
            StreamReader partReader = new StreamReader(partStream);
            while (!partReader.EndOfStream)
            {
                string line = partReader.ReadLine();
                string[] token = { "\",\"" };
                line = line.Trim('\"');
                string[] items = line.Split(token, StringSplitOptions.None);

                Part p = new Part() { DrinkId = items[0], Amount = items[1], IngredientId = items[2], IngredientName = items[3] };
                PartList.Add(p);
            }

            Drink previousDrink = null;  //this will optimize the search to 6,215 times
            foreach(Part p in PartList)
            {
                Drink foundDrink = null;
                if(previousDrink?.Id == p.DrinkId)
                {
                    foundDrink = previousDrink;
                }
                else
                {
                    var query = from d in DrinkList
                                where d.Id == p.DrinkId
                                select d;
                    if( query.Count() >= 1)
                    {
                        foundDrink = query.First();
                    }
                }

                if(foundDrink != null)
                {
                    foundDrink.Ingredients.Add(p);
                }

                previousDrink = foundDrink;
            }

            foreach(Drink d in PersonalList)
            {
                DrinkList.Add(d);
            }

            FavoritesList = new List<Drink>();
            foreach(Drink d in FavoritesDrinks)
            {
                Drink refD = DrinkList.Find(x => x.Id == d.Id);

                if (refD != null)
                {
                    if (!refD.Favorite)
                    {
                        refD.ToggleFavorite();
                    }
                    else
                    {
                        FavoritesList.Add(d);
                    }
                }
            }

        }

        public static void Save()
        {
            var localObjectStorageHelper = new LocalObjectStorageHelper();
            localObjectStorageHelper.Save<int>("LaunchCount", launchCount);
            localObjectStorageHelper.Save<List<Drink>>("PersonalDrinks", PersonalList);
            localObjectStorageHelper.Save<List<Drink>>("FavoriteDrinks", FavoritesList);
            localObjectStorageHelper.Save<List<string>>("DefaultSelectedIngredients", DefaultSelectedIngredients);
        }

        public static List<Drink> SearchByAvailableIngredients(List<string> ingredients)
        {
            IQueryable<Drink> query = DrinkList.AsQueryable<Drink>();

            query = query.Where(p => p.Ingredients.All(x => ingredients.Contains(x.IngredientName) || x.IsOptionalIngredient()));

            return query.ToList();
            
            //query = query.Where(p => p.Ingredients.Except())

            //query = query.Where(p => ingredients.Intersect(p.Ingredients).count)
        }

        public static List<Drink> Search(string name, List<string> ingredients, List<string> alcoholTypes, List<string> categories)
        {
            IQueryable<Drink> query = DrinkList.AsQueryable<Drink>();

            if(!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.ToUpper().Contains(name.ToUpper()));
            }

            if(ingredients.Count() > 0)
            {
                query = query.Where(p => p.Ingredients.Any(x => ingredients.Contains(x.IngredientName)));
            }

            if(alcoholTypes.Count() > 0)
            {
                query = query.Where(p => alcoholTypes.Contains(p.Alcohol));
            }

            if(categories.Count() > 0)
            {
                query = query.Where(p => categories.Contains(p.Category));
            }

            return query.ToList();
        }

        public static List<Drink> PersonalList { get; set; }
        public static List<Drink> FavoritesList { get; set; }
        public static List<Drink> DrinkList { get; set; }
        public static List<Ingredient> IngredientList { get; set; }
        public static List<string> DrinkCategories { get; set; }
        public static List<string> AlcoholTypes { get; set; }
        public static List<Part> PartList { get; set; }
        public static List<string> DefaultSelectedIngredients { get; set; }

    }

    public class ToggleFavoriteCommand : ICommand
    {
        private Drink drink;

        public ToggleFavoriteCommand(Drink drinkItem)
        {
            drink = drinkItem;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            drink.ToggleFavorite();
        }
    }

    public class Drink : INotifyPropertyChanged
    {
        public Drink()
        {
            Ingredients = new List<Part>();
            Favorite = false;
            ToggleFavoriteCommand = new ToggleFavoriteCommand(this);
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public string Instructions { get; set; }
        public string Category { get; set; }
        public string Alcohol { get; set; }
        public string ServeIn { get; set; }

        public List<Part> Ingredients { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool favorite;
        public bool Favorite
        {
            get { return favorite; }
            set
            {
                favorite = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Favorite"));
            }
        }

public ToggleFavoriteCommand ToggleFavoriteCommand { get; set; } 
        public void ToggleFavorite()
        {
            Favorite = !Favorite;
            if (Favorite)
            {
                DrinkManager.FavoritesList.Add(this);
            }
            else
            {
                DrinkManager.FavoritesList.Remove(this);
            }
        }

    }

    public class Part
    {
        public string DrinkId { get; set; }
        public string Amount { get; set; }
        public string IngredientId { get; set; }
        public string IngredientName { get; set; }

        public bool IsOptionalIngredient()
        {
            Ingredient ing = DrinkManager.IngredientList.Find(x => x.Id == this.IngredientId);

            if (ing.Category != "Other / Unknown" &&
                  ing.Category != "Herb / Spice" &&
                   ing.Category != "Garnish")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    //ideally this should be normalized
    public class RelatedIngredient
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class Ingredient
    { 

        public Ingredient()
        {
            RelatedIngredients = new List<RelatedIngredient>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Id { get; set; }
        public string Category { get; set; }
        public string Alcohol { get; set; }
        public List<RelatedIngredient> RelatedIngredients { get; set; }

        public static explicit operator Ingredient(List<object> v)
        {
            throw new NotImplementedException();
        }
    }

}
