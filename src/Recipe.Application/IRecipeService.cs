namespace Recipe.Application;

public interface IRecipeService
{
    public Task<IEnumerable<Core.Recipe>> GetRecipesAsync();
}