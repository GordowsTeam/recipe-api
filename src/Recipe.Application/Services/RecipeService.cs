using System.Linq;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IThirdPartyRecipeService _thirdPartyService;

    public RecipeService(IRecipeRepository recipeRepository, IThirdPartyRecipeService thirdPartyService)
    {
        _recipeRepository = recipeRepository;
        _thirdPartyService = thirdPartyService;
    }
    
    public async Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request)
    {
        var result = new List<RecipeResponse>();
        result.AddRange(await _recipeRepository.GetRecipesAsync(request) ?? []);
        result.AddRange(await _thirdPartyService.GetRecipesAsync(request) ?? []);

        return result;
    }
}