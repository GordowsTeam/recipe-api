namespace Recipe.Application.Interfaces;

public interface IAddMyRecipeUseCase
{
    Task ExecuteAsync(string recipeId, Domain.Enums.RecipeSourceType recipeSourceType, CancellationToken ct = default);
}
