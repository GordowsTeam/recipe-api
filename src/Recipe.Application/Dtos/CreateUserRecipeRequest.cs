namespace Recipe.Application.Dtos;

public class CreateUserRecipeRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Ingredients { get; set; } = [];
    public List<string> Directions { get; set; } = [];
    public decimal TotalTime { get; set; }
    public decimal Calories { get; set; }
}
