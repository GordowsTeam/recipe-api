using Microsoft.Extensions.Logging;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Recipe.Domain.Enums;

namespace Recipe.Application.Services;

public class GetMyRecipesUseCase(
    ICurrentUserService currentUserService,
    IUserRecipeRepository userRecipeRepository,
    IUserCreatedRecipeRepository userCreatedRecipeRepository,
    IGetRecipeUseCase getRecipeUseCase,
    ILogger<GetMyRecipesUseCase> logger) : IGetMyRecipesUseCase
{
    public async Task<IReadOnlyList<RecipeDetailResponse>> ExecuteAsync(CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Array.Empty<RecipeDetailResponse>();

        var results = new List<RecipeDetailResponse>();

        // Saved recipes (from search)
        var refs = await userRecipeRepository.GetByUserIdAsync(userId, ct);
        foreach (var r in refs)
        {
            try
            {
                var recipe = await getRecipeUseCase.ExecuteAsync(r.RecipeId, r.RecipeSourceType, Language.Spanish);
                if (recipe != null)
                    results.Add(recipe);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Could not load recipe {RecipeId} for user recipes", r.RecipeId);
            }
        }

        // User-created recipes
        var created = await userCreatedRecipeRepository.GetByUserIdAsync(userId, ct);
        results.AddRange(created);

        return results;
    }
}
