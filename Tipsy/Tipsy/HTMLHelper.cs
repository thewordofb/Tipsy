using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tipsy
{
    public static class HTMLHelper
    {
        public static string Generate(Drink drink)
        {
            string html = string.Empty;
          //  string html = "<!DOCTYPE html><html><body>";
            
            html += "<h1>" + drink.Name + "</h1>";
            html += "<p>Type: " + drink.Category + "<br>Alcohol: " + drink.Alcohol + "</p>";

            html += "<p>";
            foreach (var part in drink.Ingredients)
            {
                html +=  part.Amount + " " + part.IngredientName + "<br>"; 
            }
            html += "</p>";

            html += "<p>" + drink.Instructions + "</p>";


           // html += "</body></html>";
            return Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.CreateHtmlFormat(html);
        }
    }
}
