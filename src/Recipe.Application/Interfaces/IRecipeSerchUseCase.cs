using Recipe.Application.Dtos;
using Recipe.Domain.Enums;

namespace Recipe.Application.Interfaces;
public interface IRecipeSearchUseCase
{
    public Task<IEnumerable<RecipeListResponse>?> ExecuteAsync(RecipeRequest request, RecipeSourceType recipeSourceType);
}