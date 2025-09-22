using System.Collections.Generic;
using Recipe.Domain;
using Recipe.Domain.Models;
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
                new() { Url = "http://example.com/image.jpg", Main = true }
            };

            // Act
            var recipeResponse = new Domain.Models.Recipe
            {
                Name = name,
                Images = images
            };

            // Assert
            Assert.Equal(name, recipeResponse.Name);
            Assert.Equal(images, recipeResponse.Images);
        }
    }
}