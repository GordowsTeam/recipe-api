namespace Recipe.Application.Dtos;

public class RecipeRequest
{
    public RecipeRequest(IEnumerable<string> ingredients = null)
    {
        Ingredients = ingredients;
    }

    public IEnumerable<string>? Ingredients { get; set; }    
}
