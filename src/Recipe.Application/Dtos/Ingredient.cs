using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class Ingredient
{
    /// <summary>Reference to the ingredient catalog entry (Recipe.Domain.Models.Inventory.Ingredient.Id).</summary>
    public Guid? IngredientId { get; set; }
    public string? Text { get; set; }//TODO: Change for Name
    public decimal Quantity { get; set; }
    public string? Measure { get; set; }
    public decimal Weight { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? Image { get; set; }
}
