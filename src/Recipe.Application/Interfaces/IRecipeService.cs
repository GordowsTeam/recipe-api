using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;
public interface IRecipeService
{
    public Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request);
    public Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id);
}