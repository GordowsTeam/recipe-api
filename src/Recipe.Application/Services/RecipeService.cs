using System.Linq;
using Microsoft.Extensions.Logging;
using Recipe.Application.Constants;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IThirdPartyRecipeService _thirdPartyService;
    private readonly string _useThirdPartyServices;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(IRecipeRepository recipeRepository, IThirdPartyRecipeService thirdPartyService, ILogger<RecipeService> logger)
    {
        _recipeRepository = recipeRepository;
        _thirdPartyService = thirdPartyService;
        _useThirdPartyServices = Environment.GetEnvironmentVariable(Common.EDAMAME_API_ACTIVE) ?? "false";
        _logger = logger;
    }
    
    public async Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request)
    {
        _logger.LogInformation("My first log in CloudWatch");
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