using Recipe.Application.Interfaces;

namespace Recipe.Application.Services;

public class RemoveFavoriteRecipeUseCase(
    ICurrentUserService currentUserService,
    IFavoriteRecipeRepository favoriteRecipeRepository) : IRemoveFavoriteRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is required to remove a favorite.");

        await favoriteRecipeRepository.RemoveAsync(userId, recipeId, ct);
    }
}
