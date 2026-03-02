namespace Recipe.Infrastructure.MongoDBRepo;

public class UserCreatedRecipeDocument
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Ingredients { get; set; } = [];
    public List<string> Directions { get; set; } = [];
    public decimal TotalTime { get; set; }
    public decimal Calories { get; set; }
    public DateTime CreatedAt { get; set; }
}
