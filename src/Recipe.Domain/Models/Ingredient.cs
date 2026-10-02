using MongoDB.Bson.Serialization.Attributes;
using Recipe.Domain.Enums;

namespace Recipe.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class Ingredient
    {
        /// <summary>Reference to Recipe.Domain.Models.Inventory.Ingredient.Id in the ingredient catalog.</summary>
        public Guid? IngredientId { get; set; }
        public string? Name { get; set; }
        public decimal Quantity { get; set; } // in units of Measure, ie. 100, 200, 1
        public string? Measure { get; set; } // unit of measure, ie. grams, ml, cup, tbsp, tsp
        public decimal Weight { get; set; } //check this maybe remove
        public FoodCategory FoodCategory { get; set; }
        public string? Image { get; set; }
    }
}
