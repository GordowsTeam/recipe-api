using Microsoft.Extensions.Logging;
using Recipe.Application.Interfaces;
using Recipe.Application.Dtos;
using Recipe.Domain.Enums;
using Recipe.Core.Enums;

namespace Recipe.Application.Services;
public interface IGetRecipeUseCase 
{
    Task<RecipeDetailResponse?> ExecuteAsync(string recipeId, RecipeSourceType recipeSourceType, Language language);
}

public class GetRecipeUseCase: IGetRecipeUseCase
{
    private readonly IRecipeServiceFactory _recipeServiceFactory;
    private readonly ILogger<GetRecipeUseCase> _logger;

    public GetRecipeUseCase(IRecipeServiceFactory recipeServiceFactory,
                         ILogger<GetRecipeUseCase> logger)
    {
        _recipeServiceFactory = recipeServiceFactory;
        _logger = logger;
    }
    
    public async Task<RecipeDetailResponse?> ExecuteAsync(string recipeId, RecipeSourceType recipeSourceType, Language language)
    {
        if (string.IsNullOrEmpty(recipeId))
        {
            throw new ArgumentException("Recipe Id cannot be null or empty");
        }

        var service = _recipeServiceFactory.CreateRecipeService(recipeSourceType);
        var result = await service.GetRecipeByIdAsync(recipeId, language);

        return result;
    }
}