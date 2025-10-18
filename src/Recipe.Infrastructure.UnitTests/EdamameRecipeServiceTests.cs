using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;
using Recipe.Infrastructure.Services.Edamame;
using Recipe.Infrastructure.Services.Edamame.Dtos;
using Xunit;

namespace Recipe.Infrastructure.Tests.Services.Edamame
{
    public class EdamameRecipeServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IOptions<EdamameAPISettings>> _edamameApiSettingsMock;
        private readonly Mock<ILogger<EdamameRecipeService>> _loggerMock;
        private readonly EdamameRecipeService _edamameRecipeService;
        private readonly EdamameAPISettings _edamameApiSettings;

        public EdamameRecipeServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _edamameApiSettingsMock = new Mock<IOptions<EdamameAPISettings>>();
            _loggerMock = new Mock<ILogger<EdamameRecipeService>>();

            _edamameApiSettings = new EdamameAPISettings
            {
                AppId = "testAppId",
                AppKey = "testAppKey",
                Uri = "http://testuri.com",
                Beta = "testBeta",
                Type = "testType"
            };

            _edamameApiSettingsMock.Setup(x => x.Value).Returns(_edamameApiSettings);

            _edamameRecipeService = new EdamameRecipeService(_httpClientFactoryMock.Object, _edamameApiSettingsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetRecipesAsync_ShouldReturnRecipes_WhenResponseIsSuccessful()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
            var edamameRecipeResponse = new EdamameRecipeResponse(new[]
            {
                new Source(new EdamameRecipe(
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
                ))
            });

            var httpClientHandlerMock = new Mock<HttpMessageHandler>();
            httpClientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(edamameRecipeResponse))
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri(_edamameApiSettings.Uri)
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _edamameRecipeService.GetRecipesAsync(request);

            // Assert
            Assert.NotNull(result);
            var recipeResponse = Assert.Single(result);
            Assert.Equal("Test Recipe", recipeResponse.Name);
        }

        [Fact]
        public async Task GetRecipesAsync_ShouldReturnNull_WhenResponseIsUnsuccessful()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };

            var httpClientHandlerMock = new Mock<HttpMessageHandler>();
            httpClientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent("Bad Request")
                });

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri(_edamameApiSettings.Uri)
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _edamameRecipeService.GetRecipesAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRecipesAsync_ShouldReturnNull_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };

            var httpClientHandlerMock = new Mock<HttpMessageHandler>();
            httpClientHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Request failed"));

            var httpClient = new HttpClient(httpClientHandlerMock.Object)
            {
                BaseAddress = new Uri(_edamameApiSettings.Uri)
            };

            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _edamameRecipeService.GetRecipesAsync(request);

            // Assert
            Assert.Null(result);
        }
    }
}