using Microsoft.Extensions.Logging;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Recipe.Domain.Enums;

namespace Recipe.Application.Services;

public class GetFavoriteRecipesUseCase(
    ICurrentUserService currentUserService,
    IFavoriteRecipeRepository favoriteRecipeRepository,
    IGetRecipeUseCase getRecipeUseCase,
    ILogger<GetFavoriteRecipesUseCase> logger) : IGetFavoriteRecipesUseCase
{
    public async Task<IReadOnlyList<RecipeDetailResponse>> ExecuteAsync(CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Array.Empty<RecipeDetailResponse>();

        var refs = await favoriteRecipeRepository.GetByUserIdAsync(userId, ct);
        var results = new List<RecipeDetailResponse>();

        foreach (var r in refs)
        {
            try
            {
                var recipe = await getRecipeUseCase.ExecuteAsync(r.RecipeId, RecipeSourceType.Internal, Language.Spanish);
                if (recipe != null)
                    results.Add(recipe);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not load recipe {RecipeId} for favorites", r.RecipeId);
            }
        }

        return results;
    }
}
