using Recipe.Domain.Enums;

namespace Recipe.Domain.Models
{
    public class GlobalIngredient
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Image { get; set; }
        public FoodCategory FoodCategory { get; set; }
        public string? FoodCategoryId { get; set; }
        public Nutrients Nutrients { get; set; } = new();
    }
}
