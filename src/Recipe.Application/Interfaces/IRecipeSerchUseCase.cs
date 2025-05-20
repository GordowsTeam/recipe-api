using Recipe.Core.Models;
using Recipe.Core.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeSearchUseCase
{
    public Task<IEnumerable<RecipeListResponse>?> ExecuteAsync(RecipeRequest request, RecipeSourceType recipeSourceType);
}