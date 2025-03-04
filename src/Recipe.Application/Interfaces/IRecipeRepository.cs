using Recipe.Core.Models;
namespace Recipe.Application.Interfaces;

public interface IRecipeRepository
{
    Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request);
}