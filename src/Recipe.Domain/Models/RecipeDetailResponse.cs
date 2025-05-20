using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Recipe.Core.Enums;

namespace Recipe.Core.Models
{
    public class RecipeDetailResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<Image>? Images { get; set; }
        public IEnumerable<Ingredient>? Ingredients { get; set; }
        public IEnumerable<string>? MissingIngredients { get; set; }//TODO:check this
        public decimal Calories { get; set; }
        public decimal TotalTime { get; set; }
        public IEnumerable<string>? CuisinTypes { get; set; }
        public IEnumerable<string>? MealTypes { get; set; }
        public IEnumerable<Direction>? Directions { get; set; }
        public RecipeSourceType RecipeSourceType { get; set; }
    }
}
