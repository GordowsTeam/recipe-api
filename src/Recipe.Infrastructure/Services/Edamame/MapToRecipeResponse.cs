using Recipe.Core.Models;

namespace Recipe.Infrastructure.Services.Edamame;

public static class MapToRecipeResponse
{
    public static IEnumerable<RecipeResponse> Map(this EdamameRecipeResponse response)
    {
        var result = new List<RecipeResponse>();
        if (response?.Hits == null) 
        {
            return result;
        }

        foreach (var edamameRecipe in response.Hits)
        {
            if (edamameRecipe.Recipe == null) continue;
            
            result.Add(new RecipeResponse()
            {
                Name = edamameRecipe.Recipe.Label ?? string.Empty,
                Images = [new() { Url = edamameRecipe.Recipe.Image ?? string.Empty, Main = true }],
                Ingredients = edamameRecipe.Recipe.Ingredients == null ? [] : edamameRecipe.Recipe.Ingredients.Select(l => new Ingredient() { Text = l.Text, Quantity = l.Quantity, Measure = l.Measure, Food = l.Food, Weight = l.Weight, Image = l.Image }),
                MissingIngredients = [], //TODO
                Calories = edamameRecipe.Recipe.Calories,
                TotalTime = edamameRecipe.Recipe.TotalTime,
                CuisinTypes = edamameRecipe.Recipe?.CuisineType == null ? [] : edamameRecipe.Recipe.CuisineType,
                MealTypes = edamameRecipe.Recipe?.MealType ?? [],
                Directions = []//TODO

            });
        }
        return result;
    }
}