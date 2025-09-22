using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class Ingredient
{
    public Guid? GlobalIngredientId { get; set; }  // reference to global ingredient that contains the metadata
    public string? Text { get; set; }//TODO: Change for Name
    public decimal Quantity { get; set; }
    public string? Measure { get; set; }
    public decimal Weight { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? Image { get; set; }
}
