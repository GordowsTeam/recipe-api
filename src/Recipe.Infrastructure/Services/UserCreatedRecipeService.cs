using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;

namespace Recipe.Infrastructure.Services;

public class UserCreatedRecipeService : IRecipeService
{
    private readonly IUserCreatedRecipeRepository _repository;

    public UserCreatedRecipeService(IUserCreatedRecipeRepository repository)
    {
        _repository = repository;
    }

    public Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id, Language language = Language.Spanish)
        => _repository.GetByIdAsync(id);

    public Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request)
        => Task.FromResult<IEnumerable<RecipeListResponse>?>(Array.Empty<RecipeListResponse>());
}
