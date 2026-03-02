using Recipe.Application.Interfaces;

namespace Recipe.Application.Services;

public class RemoveMyRecipeUseCase(
    ICurrentUserService currentUserService,
    IUserRecipeRepository userRecipeRepository,
    IUserCreatedRecipeRepository userCreatedRecipeRepository) : IRemoveMyRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is required to remove a recipe.");

        await userRecipeRepository.RemoveAsync(userId, recipeId, ct);
        await userCreatedRecipeRepository.DeleteAsync(userId, recipeId, ct);
    }
}
