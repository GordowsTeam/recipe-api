using Recipe.Core.Enums;

namespace Recipe.Application.Dtos;

public class RecipeRequest
{
    public RecipeRequest(IEnumerable<string>? ingredients = null)
    {
        Ingredients = ingredients;
    }

    public IEnumerable<string>? Ingredients { get; set; }
    public bool MatchAllIngredients { get; set; } = false;
    public string? Name { get; set; }
    public IEnumerable<string>? CuisineTypes { get; set; }
    public IEnumerable<string>? MealTypes { get; set; }
    public int NumberOfRecipes { get; set; } = 10;
    public Language Language { get; set; } = Language.Spanish;
}
