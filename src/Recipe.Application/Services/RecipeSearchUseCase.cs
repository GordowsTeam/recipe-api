using Microsoft.Extensions.Logging;
using Recipe.Application.Constants;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;
using Recipe.Core.Enums;

namespace Recipe.Application.Services;
public class RecipeSearchUseCase: IRecipeSearchUseCase
{
    private readonly IRecipeServiceFactory _recipeServiceFactory;
    private readonly ILogger<RecipeSearchUseCase> _logger;

    public RecipeSearchUseCase(IRecipeServiceFactory recipeServiceFactory,
                         ILogger<RecipeSearchUseCase> logger)
    {
        _recipeServiceFactory = recipeServiceFactory;
        _logger = logger;
    }
    
    public async Task<IEnumerable<RecipeListResponse>> ExecuteAsync(RecipeRequest recipeRequest, RecipeSourceType recipeSourceType)
    {
        ArgumentNullException.ThrowIfNull(recipeRequest);

        if (recipeRequest.Ingredients == null || !recipeRequest.Ingredients.Any() || recipeRequest.Ingredients.Any(l => string.IsNullOrEmpty(l)))
        {
            throw new ArgumentException("Ingredients cannot be null or empty", nameof(recipeRequest.Ingredients));
        }

        var service = _recipeServiceFactory.CreateRecipeService(recipeSourceType);
        var result = await service.GetRecipesAsync(recipeRequest);
        return result;
    }
}