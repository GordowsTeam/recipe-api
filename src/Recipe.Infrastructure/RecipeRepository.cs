using Recipe.Application;

namespace Recipe.Infrastructure;

public class RecipeRepository : IRecipeRepository
{
    public Task<IEnumerable<Core.Recipe>> GetRecipesAsync()
    {
        var recipes = new List<Recipe.Core.Recipe>() { new ()
        {
            Name = "Pasta",
            Images = new List<Recipe.Core.Image>() { new() { Url = "https://www.example.com/pasta.jpg", Main = true } },
            Ingredients = new List<Recipe.Core.Ingredient>() { new() { Text = "Pasta", Quantity = "1", Measure = "cup", Weight = 100, Food = "Pasta", FoodCategory = Recipe.Core.FoodCategory.None, FoodCategoryId = "1", Image = "https://www.example.com/pasta.jpg" } },
            MissingIngredients = new List<string>() { "Tomato", "Cheese" },
            Calories = 100,
            TotalTime = 30,
            CuisinTypes = new List<string>() { "Italian" },
            MealTypes = new List<string>() { "Lunch" },
            Directions = new List<Recipe.Core.Direction>() { new() { Step = "1", Image = "https://www.example.com/pasta.jpg", InstructionText = "Boil Pasta" } }
        } };
        return Task.FromResult<IEnumerable<Core.Recipe>>(recipes);
    }
}