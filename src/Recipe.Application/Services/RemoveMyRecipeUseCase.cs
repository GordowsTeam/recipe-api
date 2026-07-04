using Recipe.Application.Interfaces;

namespace Recipe.Application.Services;

public class RemoveMyRecipeUseCase(
    ICurrentUserService currentUserService,
    IUserRecipeRepository userRecipeRepository,
    IUserCreatedRecipeRepository userCreatedRecipeRepository) : IRemoveMyRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, CancellationToken ct = default)
    {
        var userEmail = currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            throw new UnauthorizedAccessException("User email is required to remove a recipe.");

        await userRecipeRepository.RemoveAsync(userEmail, recipeId, ct);
        await userCreatedRecipeRepository.DeleteAsync(userEmail, recipeId, ct);
    }
}
