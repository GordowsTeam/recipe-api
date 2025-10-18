using Microsoft.Extensions.Logging;
using Recipe.Application.Interfaces;
using Recipe.Application.Dtos;
using Recipe.Domain.Enums;
using Recipe.Application.Validators;

namespace Recipe.Application.Services;
public class RecipeSearchUseCase(IRecipeServiceFactory recipeServiceFactory,
                     ILogger<RecipeSearchUseCase> logger) : IRecipeSearchUseCase
{
    private readonly IRecipeServiceFactory _recipeServiceFactory = recipeServiceFactory;
    private readonly ILogger<RecipeSearchUseCase> _logger = logger;

    public async Task<IEnumerable<RecipeListResponse>?> ExecuteAsync(RecipeRequest recipeRequest, RecipeSourceType recipeSourceType)
    {
        ArgumentNullException.ThrowIfNull(recipeRequest);

        if (!recipeRequest.IsValid(out var errorMessage))
            throw new ArgumentException(errorMessage);

        var service = _recipeServiceFactory.CreateRecipeService(recipeSourceType);
        var result = await service.GetRecipesAsync(recipeRequest);

        return result;
    }
}