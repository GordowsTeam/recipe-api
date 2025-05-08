using Recipe.Core.Models;
using Recipe.Core.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeService
{
    public Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request);
    public Task<RecipeResponse> GetRecipeByIdAsync(string id);
}