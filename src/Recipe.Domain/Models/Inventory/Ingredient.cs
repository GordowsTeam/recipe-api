using Recipe.Domain.Enums;

namespace Recipe.Domain.Models.Inventory;

public class Ingredient
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public FoodCategory FoodCategory { get; set; }
    public string? Image { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
