using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipe.Infrastructure.Services.Edamame.Dtos
{
    public record EdamameRecipeResponse(Source[]? Hits);

    public record Source(EdamameRecipe? Recipe);

    public record EdamameRecipe(string? Uri, string? Label, string? Url, string[]? IngredientLines, string? Image, EdamameIngredient[]? Ingredients, decimal Calories, decimal TotalTime, string[]? CuisineType, string[]? MealType);

    public record EdamameIngredient(string Text, decimal Quantity, string Measure, string Food, decimal Weight, string Image);
}
