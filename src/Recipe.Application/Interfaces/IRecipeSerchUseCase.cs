using Recipe.Core.Models;
using Recipe.Core.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeSearchUseCase
{
    public Task<IEnumerable<RecipeResponse>> ExecuteAsync(RecipeRequest request, RecipeSourceType recipeSourceType);
}