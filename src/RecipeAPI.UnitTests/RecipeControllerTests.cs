using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;
using RecipeAPI.Controllers;
using Xunit;

namespace RecipeAPI.UnitTests
{
    public class RecipeControllerTests
    {
        private readonly Mock<IRecipeService> _recipeServiceMock;
        private readonly Mock<ILogger<RecipeController>> _loggerMock;
        private readonly RecipeController _recipeController;

        public RecipeControllerTests()
        {
            _recipeServiceMock = new Mock<IRecipeService>();
            _loggerMock = new Mock<ILogger<RecipeController>>();
            _recipeController = new RecipeController(_recipeServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = await _recipeController.Post(null, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Invalid request. Ingredients are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenIngredientsAreNull()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = null };

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Invalid request. Ingredients are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenIngredientsAreEmpty()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = Enumerable.Empty<string>() };

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Invalid request. Ingredients are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
            var response = new List<RecipeResponse>
            {
                new RecipeResponse { Name = "Tomato Soup" }
            };
            _recipeServiceMock.Setup(service => service.GetRecipesAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedResponse = Assert.IsType<List<RecipeResponse>>(okResult.Value);
            Assert.Equal(response, returnedResponse);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
            _recipeServiceMock.Setup(service => service.GetRecipesAsync(request)).ThrowsAsync(new ArgumentException("Invalid request. Ingredients are required."));

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Invalid request. Ingredients are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
            _recipeServiceMock.Setup(service => service.GetRecipesAsync(request)).ThrowsAsync(new Exception("An error occurred while processing the request."));

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeResponse>>(result);
            var statusCodeResult = Assert.IsType<ObjectResult>(actionResult.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while processing your request.", statusCodeResult.Value);
        }
    }
}