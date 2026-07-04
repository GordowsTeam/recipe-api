using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;

namespace Recipe.Application.Services;

public class AddMyRecipeUseCase(
    ICurrentUserService currentUserService,
    IUserRecipeRepository userRecipeRepository) : IAddMyRecipeUseCase
{
    public async Task ExecuteAsync(string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default)
    {
        var userEmail = currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            throw new UnauthorizedAccessException("User email is required to add a recipe.");

        await userRecipeRepository.AddAsync(userEmail, recipeId, recipeSourceType, ct);
    }
}
