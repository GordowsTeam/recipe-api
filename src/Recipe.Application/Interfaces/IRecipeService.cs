using Recipe.Core.Models;

namespace Recipe.Application.Interfaces;

public interface IRecipeService
{
    public Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request);
}