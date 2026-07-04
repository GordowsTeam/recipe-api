namespace Recipe.Application.Interfaces;

public interface IRemoveFavoriteRecipeUseCase
{
    Task ExecuteAsync(string recipeId, CancellationToken ct = default);
}
