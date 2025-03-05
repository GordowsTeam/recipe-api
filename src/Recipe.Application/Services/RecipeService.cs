using System.Linq;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IThirdPartyRecipeService _thirdPartyService;
    private readonly string _useThirdPartyServices;

    public RecipeService(IRecipeRepository recipeRepository, IThirdPartyRecipeService thirdPartyService)
    {
        _recipeRepository = recipeRepository;
        _thirdPartyService = thirdPartyService;
        _useThirdPartyServices = Environment.GetEnvironmentVariable("EDAMAME_API_ACTIVE") ?? "false";
    }
    
    public async Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request)
    {
        var result = new List<RecipeResponse>();
        var recipesFromDB = await _recipeRepository.GetRecipesAsync(request);
        if (recipesFromDB != null) 
        {
            result.AddRange(recipesFromDB);
        }


        if (_useThirdPartyServices == "true")
        {
            var recipesFromThirdParty = await _thirdPartyService.GetRecipesAsync(request);
            if (recipesFromThirdParty != null)
            {
                result.AddRange(recipesFromThirdParty);
            }
        }

        return result;
    }
}