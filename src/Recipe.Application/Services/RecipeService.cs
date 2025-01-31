using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }
    
    public Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request)
    {
        return _recipeRepository.GetRecipesAsync();
    }
}