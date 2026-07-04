using Recipe.Domain.Enums;

namespace Recipe.Application.Dtos;

public class AddMyRecipeRequest
{
    public required string RecipeId { get; set; }
    public RecipeSourceType RecipeSourceType { get; set; }
}
