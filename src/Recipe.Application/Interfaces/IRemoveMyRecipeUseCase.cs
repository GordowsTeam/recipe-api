namespace Recipe.Application.Interfaces;

public interface IRemoveMyRecipeUseCase
{
    Task ExecuteAsync(string recipeId, CancellationToken ct = default);
}
