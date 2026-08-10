using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class IngredientResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? Image { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
