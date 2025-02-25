using Recipe.Application.Interfaces;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services;

public class RecipeRepository : IRecipeRepository
{
    public Task<IEnumerable<RecipeResponse>> GetRecipesAsync()
    {
        var recipes = new List<RecipeResponse>() { new ()
        {
            Name = "Pasta 4",
            Images = new List<Image>() { new() { Url = "https://www.example.com/pasta.jpg", Main = true } },
            Ingredients = new List<Ingredient>() { new() { Text = "Pasta", Quantity = "1", Measure = "cup", Weight = 100, Food = "Pasta", FoodCategory = FoodCategory.None, FoodCategoryId = "1", Image = "https://www.example.com/pasta.jpg" } },
            MissingIngredients = new List<string>() { "Tomato", "Cheese" },
            Calories = 100,
            TotalTime = 30,
            CuisinTypes = new List<string>() { "Italian" },
            MealTypes = new List<string>() { "Lunch" },
            Directions = new List<Direction>() { new() { Step = "1", Image = "https://www.example.com/pasta.jpg", InstructionText = "Boil Pasta" } }
        } };
        return Task.FromResult<IEnumerable<RecipeResponse>>(recipes);
    }
}