using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;

namespace Recipe.Application.Services;

public class AddFavoriteRecipeUseCase(
    ICurrentUserService currentUserService,
    IFavoriteRecipeRepository favoriteRecipeRepository) : IAddFavoriteRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default)
    {
        var userEmail = currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            throw new UnauthorizedAccessException("User email is required to add a favorite.");

        await favoriteRecipeRepository.AddAsync(userEmail, recipeId, recipeSourceType, ct);
    }
}
