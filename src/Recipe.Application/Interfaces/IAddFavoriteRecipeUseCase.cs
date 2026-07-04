namespace Recipe.Application.Interfaces;

public interface IAddFavoriteRecipeUseCase
{
    Task ExecuteAsync(string recipeId, Domain.Enums.RecipeSourceType recipeSourceType, CancellationToken ct = default);
}
