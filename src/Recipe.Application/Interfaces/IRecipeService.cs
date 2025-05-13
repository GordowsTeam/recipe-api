using Recipe.Core.Models;
using Recipe.Core.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeService
{
    public Task<IEnumerable<RecipeListResponse>> GetRecipesAsync(RecipeRequest request);
    public Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id);
}