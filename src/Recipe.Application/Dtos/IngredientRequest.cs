using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class IngredientRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? Image { get; set; }
}
