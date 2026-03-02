using Recipe.Domain.Enums;

namespace Recipe.Infrastructure.MongoDBRepo;

public class FavoriteRecipeDocument
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string RecipeId { get; set; } = string.Empty;
    public RecipeSourceType RecipeSourceType { get; set; }
    public DateTime AddedAt { get; set; }
}
