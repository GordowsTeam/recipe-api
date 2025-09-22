using Recipe.Domain.Enums;

namespace Recipe.Domain.Models
{
    public class Ingredient
    {
        public Guid? GlobalIngredientId { get; set; }  // reference to global ingredient that contains the metadata
        public string? Name { get; set; }
        public decimal Quantity { get; set; } // in units of Measure, ie. 100, 200, 1
        public string? Measure { get; set; } // unit of measure, ie. grams, ml, cup, tbsp, tsp
        public decimal Weight { get; set; } //check this maybe remove
        public FoodCategory FoodCategory { get; set; }
        public string? Image { get; set; }
    }
}
