using Recipe.Domain.Enums;

namespace Recipe.Application.Interfaces;

public interface IUserRecipeRepository
{
    Task<IReadOnlyList<UserRecipeRef>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(string userId, string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default);
    Task RemoveAsync(string userId, string recipeId, CancellationToken ct = default);
}

public record UserRecipeRef(string RecipeId, RecipeSourceType RecipeSourceType);
