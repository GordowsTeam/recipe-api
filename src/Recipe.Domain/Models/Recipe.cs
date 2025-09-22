using Recipe.Domain.Enums;

namespace Recipe.Domain.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public IEnumerable<Image>? Images { get; set; }
        public IEnumerable<Ingredient>? Ingredients { get; set; }
        public decimal Calories { get; set; }
        public decimal TotalTime { get; set; }// in minutes
        public IEnumerable<string>? CuisinTypes { get; set; }
        public IEnumerable<string>? MealTypes { get; set; }
        public IEnumerable<Direction>? Directions { get; set; }
        public RecipeSourceType RecipeSourceType { get; set; }
    }
}
