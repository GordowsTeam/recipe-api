using Recipe.Application.Interfaces;

namespace Recipe.Application.Services;

public class RemoveFavoriteRecipeUseCase(
    ICurrentUserService currentUserService,
    IFavoriteRecipeRepository favoriteRecipeRepository) : IRemoveFavoriteRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, CancellationToken ct = default)
    {
        var userEmail = currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            throw new UnauthorizedAccessException("User email is required to remove a favorite.");

        await favoriteRecipeRepository.RemoveAsync(userEmail, recipeId, ct);
    }
}
