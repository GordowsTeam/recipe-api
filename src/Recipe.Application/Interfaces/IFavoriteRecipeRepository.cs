using Recipe.Domain.Enums;

namespace Recipe.Application.Interfaces;

public interface IFavoriteRecipeRepository
{
    Task<IReadOnlyList<FavoriteRecipeRef>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(string userId, string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default);
    Task RemoveAsync(string userId, string recipeId, CancellationToken ct = default);
}

public record FavoriteRecipeRef(string RecipeId, RecipeSourceType RecipeSourceType);
