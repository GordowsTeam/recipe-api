using System.Collections.Generic;
using Recipe.Core.Models;
using Recipe.Infrastructure.Services.Edamame;
using Xunit;

namespace Recipe.Infrastructure.Tests.Services.Edamame
{
    public class EdamameRecipeResponseExtensionsTests
    {
        [Fact]
        public void Map_ShouldReturnEmptyList_WhenResponseIsNull()
        {
            // Arrange
            EdamameRecipeResponse response = null;

            // Act
            var result = response.Map();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Map_ShouldReturnEmptyList_WhenHitsAreNull()
        {
            // Arrange
            var response = new EdamameRecipeResponse(null);

            // Act
            var result = response.Map();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Map_ShouldReturnEmptyList_WhenRecipeIsNull()
        {
            // Arrange
            var response = new EdamameRecipeResponse(new[] { new Source(null) });

            // Act
            var result = response.Map();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Map_ShouldMapEdamameRecipeResponseToRecipeResponse()
        {
            // Arrange
            var edamameRecipe = new EdamameRecipe(
                Uri: "http://example.com/recipe",
                Label: "Test Recipe",
                Url: "http://example.com/recipe",
                IngredientLines: new[] { "1 cup of sugar", "2 cups of flour" },
                Image: "http://example.com/image.jpg",
                Ingredients: new[]
                {
                    new EdamameIngredient("sugar", 1, "cup", "sugar", 200, "http://example.com/sugar.jpg"),
                    new EdamameIngredient("flour", 2, "cups", "flour", 400, "http://example.com/flour.jpg")
                },
                Calories: 500,
                TotalTime: 30,
                CuisineType: new[] { "American" },
                MealType: new[] { "Dinner" }
            );
            var response = new EdamameRecipeResponse(new[] { new Source(edamameRecipe) });

            // Act
            var result = response.Map();

            // Assert
            Assert.NotNull(result);
            var recipeResponse = Assert.Single(result);
            Assert.Equal("Test Recipe", recipeResponse.Name);
            Assert.NotNull(recipeResponse.Images);
            var image = Assert.Single(recipeResponse.Images);
            Assert.Equal("http://example.com/image.jpg", image.Url);
            Assert.True(image.Main);
            Assert.NotNull(recipeResponse.Ingredients);
            Assert.Equal(2, recipeResponse.Ingredients.Count());
            Assert.Equal(500, recipeResponse.Calories);
            Assert.Equal(30, recipeResponse.TotalTime);
            Assert.NotNull(recipeResponse.CuisinTypes);
            Assert.Single(recipeResponse.CuisinTypes);
            Assert.Equal("American", recipeResponse.CuisinTypes.First());
            Assert.NotNull(recipeResponse.MealTypes);
            Assert.Single(recipeResponse.MealTypes);
            Assert.Equal("Dinner", recipeResponse.MealTypes.First());
        }
    }
}