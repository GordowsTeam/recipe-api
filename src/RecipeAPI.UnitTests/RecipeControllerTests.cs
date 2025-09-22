using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Constants;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Domain.Models;
using RecipeAPI.Controllers;
using Xunit;

namespace RecipeAPI.UnitTests
{
    public class RecipeControllerTests
    {
        private readonly Mock<ILogger<RecipeController>> _loggerMock;
        private readonly RecipeController _recipeController;
        private readonly Mock<IRecipeSearchUseCase> _recipeSearchUseCaseMock;
        private readonly Mock<IGetRecipeUseCase> _getRecipeUseCaseMock;

        public RecipeControllerTests()
        {
            _loggerMock = new Mock<ILogger<RecipeController>>();
            _recipeSearchUseCaseMock = new Mock<IRecipeSearchUseCase>();
            _getRecipeUseCaseMock = new Mock<IGetRecipeUseCase>();
            _recipeController = new RecipeController(_recipeSearchUseCaseMock.Object, _getRecipeUseCaseMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenRequestIsNull()
        {
            // Act
            RecipeRequest recipeRequest = new();
            var result = await _recipeController.Post(recipeRequest, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeListResponse>>(result);
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
            var actionResult = Assert.IsType<ActionResult<RecipeListResponse>>(result);
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
            var actionResult = Assert.IsType<ActionResult<RecipeListResponse>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal("Invalid request. Ingredients are required.", badRequestResult.Value);
        }

        [Fact]
        public async Task Post_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            Environment.SetEnvironmentVariable(Common.INTERNAL_ACTIVE, "true");
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
            var response = new List<RecipeListResponse>
            {
                new RecipeListResponse { Id = Guid.NewGuid().ToString(), Name = "Tomato Soup" }
            };
            _recipeSearchUseCaseMock.Setup(service => service.ExecuteAsync(request, Recipe.Domain.Enums.RecipeSourceType.Internal)).ReturnsAsync(response);

            // Act
            var result = await _recipeController.Post(request, CancellationToken.None);

            // Assert
            var actionResult = Assert.IsType<ActionResult<RecipeListResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedResponse = Assert.IsType<List<RecipeListResponse>>(okResult.Value);
            Assert.Equal(response, returnedResponse);
        }
    }
}