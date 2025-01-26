namespace Recipe.Application;

public interface IRecipeRepository
{
    Task<IEnumerable<Core.Recipe>> GetRecipesAsync();
}