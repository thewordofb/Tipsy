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
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tipsy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PageToPrint : Page
    {
        public RichTextBlock TextContentBlock { get; set; }

        public PageToPrint()
        {
            this.InitializeComponent();
            TextContentBlock = TextContent;
        }

        public PageToPrint(Drink drink)
        {
            InitializeComponent();
            TextContentBlock = TextContent;

            DrinkTitle.Text = drink.Name;
            DrinkType.Text = drink.Category;
            DrinkAlcohol.Text = drink.Alcohol;
            DrinkInstructions.Text = drink.Instructions;

            foreach(var part in drink.Ingredients)
            {
                string partString = part.Amount + " " + part.IngredientName + "\n";
                Ingredients.Inlines.Add(new Run() { Text = partString });
            }
        }
    }
}
