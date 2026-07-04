using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface ICreateUserRecipeUseCase
{
    Task<RecipeDetailResponse> ExecuteAsync(CreateUserRecipeRequest request, CancellationToken ct = default);
}
