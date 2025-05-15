using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services;
public class MockRecipeRepository : IRecipeService
{
    public Task<IEnumerable<RecipeListResponse>?> GetRecipesAsync(RecipeRequest request)
    {
        var recipes = new List<RecipeListResponse>() { new ()
        {
            Name = "Pasta 14",
            Images = new List<Image>() { new() { Url = "https://cdn.apartmenttherapy.info/image/upload/f_jpg,q_auto:eco,c_fill,g_auto,w_1500,ar_1:1/k%2FPhoto%2FRecipes%2F2023-01-Caramelized-Tomato-Paste-Pasta%2F06-CARAMELIZED-TOMATO-PASTE-PASTA-039", Main = true } },
            RecipeSourceType = RecipeSourceType.Internal
        } };
        return Task.FromResult<IEnumerable<RecipeListResponse>>(recipes)!;
    }

    public Task<RecipeDetailResponse?> GetRecipeByIdAsync(string id)
    {
        var recipe = new RecipeDetailResponse()
        {
            Id = 1,
            Name = "Pasta 14",
            Images = new List<Image>() { new() { Url = "https://cdn.apartmenttherapy.info/image/upload/f_jpg,q_auto:eco,c_fill,g_auto,w_1500,ar_1:1/k%2FPhoto%2FRecipes%2F2023-01-Caramelized-Tomato-Paste-Pasta%2F06-CARAMELIZED-TOMATO-PASTE-PASTA-039", Main = true } },
            Ingredients = new List<Ingredient>() { new() { Text = "Pasta", Quantity = 1, Measure = "cup", Weight = 100, Food = "Pasta", FoodCategory = FoodCategory.None, FoodCategoryId = "1", Image = "https://www.example.com/pasta.jpg" } },
            MissingIngredients = new List<string>() { "Tomato", "Cheese" },
            Calories = 100,
            TotalTime = 30,
            CuisinTypes = new List<string>() { "Italian" },
            MealTypes = new List<string>() { "Lunch" },
            Directions = new List<Direction>() { new() { Step = "1", Image = "https://www.example.com/pasta.jpg", InstructionText = "Boil Pasta" } },
            RecipeSourceType = RecipeSourceType.Internal
        };
        
        return Task.FromResult(recipe)!;
    }
}