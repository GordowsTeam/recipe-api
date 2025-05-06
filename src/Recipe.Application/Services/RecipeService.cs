using Microsoft.Extensions.Logging;
using Recipe.Application.Constants;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IThirdPartyRecipeService _thirdPartyService;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(IRecipeRepository recipeRepository,
                         IThirdPartyRecipeService thirdPartyService,
                         ILogger<RecipeService> logger)
    {
        _recipeRepository = recipeRepository;
        _thirdPartyService = thirdPartyService;
        _logger = logger;
    }
    
    public async Task<IEnumerable<RecipeResponse>> GetRecipesAsync(RecipeRequest request)
    {
        //TODO: add a factory to get recipes from DB or externalProviders
        ArgumentNullException.ThrowIfNull(request);

        if (request.Ingredients == null || !request.Ingredients.Any() || request.Ingredients.Any(l => string.IsNullOrEmpty(l)))
        {
            throw new ArgumentException("Ingredients cannot be null or empty", nameof(request.Ingredients));
        }
        var result = new List<RecipeResponse>();
        var recipesFromDB = await _recipeRepository.GetRecipesAsync(request);
        if (recipesFromDB != null)
        {
            result.AddRange(recipesFromDB);
        }

        await AddThirdPartyRecipesAsync(request, result);

        return result;
    }

    public async Task<RecipeResponse> GetRecipeByIdAsync(int id, string externalProviderId)
    {
        var result = await _recipeRepository.GetRecipeByIdAsync(id, externalProviderId);

        return result;
    }

    private async Task AddThirdPartyRecipesAsync(RecipeRequest request, List<RecipeResponse> result)
    {
        if (Environment.GetEnvironmentVariable(Common.EDAMAME_API_ACTIVE) != "true")
        {
            return;
        }

        var recipesFromThirdParty = await _thirdPartyService.GetRecipesAsync(request);
        if (recipesFromThirdParty != null)
        {
            result.AddRange(recipesFromThirdParty);
        }
    }
}