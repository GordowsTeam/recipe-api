using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;

namespace Recipe.Application.Services;

public class CreateUserRecipeUseCase(
    ICurrentUserService currentUserService,
    IUserCreatedRecipeRepository userCreatedRecipeRepository) : ICreateUserRecipeUseCase
{
    public async Task<RecipeDetailResponse> ExecuteAsync(CreateUserRecipeRequest request, CancellationToken ct = default)
    {
        var userId = currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID is required to create a recipe.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Recipe name is required.");

        var id = await userCreatedRecipeRepository.AddAsync(userId, request, ct);
        var created = await userCreatedRecipeRepository.GetByIdAsync(id, ct)
            ?? throw new InvalidOperationException("Failed to load created recipe.");
        return created;
    }
}
