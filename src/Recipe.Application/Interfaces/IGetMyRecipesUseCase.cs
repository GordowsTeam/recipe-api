using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IGetMyRecipesUseCase
{
    Task<IReadOnlyList<RecipeDetailResponse>> ExecuteAsync(CancellationToken ct = default);
}
