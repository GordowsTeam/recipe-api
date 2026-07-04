using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IUserCreatedRecipeRepository
{
    Task<RecipeDetailResponse?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<RecipeDetailResponse>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<string> AddAsync(string userId, CreateUserRecipeRequest request, CancellationToken ct = default);
    Task DeleteAsync(string userId, string recipeId, CancellationToken ct = default);
}
