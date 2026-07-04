using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IGetFavoriteRecipesUseCase
{
    Task<IReadOnlyList<RecipeDetailResponse>> ExecuteAsync(CancellationToken ct = default);
}
