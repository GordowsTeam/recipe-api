using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;

namespace Recipe.Application.Services;

public class AddFavoriteRecipeUseCase(
    ICurrentUserService currentUserService,
    IFavoriteRecipeRepository favoriteRecipeRepository) : IAddFavoriteRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is required to add a favorite.");

        await favoriteRecipeRepository.AddAsync(userId, recipeId, recipeSourceType, ct);
    }
}
