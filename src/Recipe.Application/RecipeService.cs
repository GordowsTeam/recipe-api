namespace Recipe.Application;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }
    
    public Task<IEnumerable<Core.Recipe>> GetRecipesAsync()
    {
        return _recipeRepository.GetRecipesAsync();
    }
}