using System.Collections.Generic;
using Recipe.Core.Models;
using Xunit;

namespace Recipe.Domain.Tests.Models
{
    public class RecipeResponseTests
    {
        [Fact]
        public void RecipeResponse_ShouldSetAndGetProperties()
        {
            // Arrange
            var name = "Test Recipe";
            var images = new List<Image>
            {
                new Image { Url = "http://example.com/image.jpg", Main = true }
            };
            var ingredients = new List<Ingredient>
            {
                new Ingredient { Text = "1 cup of sugar", Quantity = 1, Measure = "cup", Food = "sugar", Weight = 200, Image = "http://example.com/sugar.jpg" }
            };
            var missingIngredients = new List<string> { "flour" };
            var calories = 500m;
            var totalTime = 30m;
            var cuisineTypes = new List<string> { "American" };
            var mealTypes = new List<string> { "Dinner" };
            var directions = new List<Direction>
            {
                new Direction { Step = "1", Image = "http://example.com/step1.jpg", InstructionText = "Mix ingredients" }
            };

            // Act
            var recipeResponse = new RecipeResponse
            {
                Name = name,
                Images = images,
                Ingredients = ingredients,
                MissingIngredients = missingIngredients,
                Calories = calories,
                TotalTime = totalTime,
                CuisinTypes = cuisineTypes,
                MealTypes = mealTypes,
                Directions = directions
            };

            // Assert
            Assert.Equal(name, recipeResponse.Name);
            Assert.Equal(images, recipeResponse.Images);
            Assert.Equal(ingredients, recipeResponse.Ingredients);
            Assert.Equal(missingIngredients, recipeResponse.MissingIngredients);
            Assert.Equal(calories, recipeResponse.Calories);
            Assert.Equal(totalTime, recipeResponse.TotalTime);
            Assert.Equal(cuisineTypes, recipeResponse.CuisinTypes);
            Assert.Equal(mealTypes, recipeResponse.MealTypes);
            Assert.Equal(directions, recipeResponse.Directions);
        }
    }
}