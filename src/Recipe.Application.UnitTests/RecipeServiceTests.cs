using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Constants;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;
using Shouldly;
using Xunit;

namespace Recipe.Application.Tests.Services;

public class RecipeServiceTests
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly Mock<IThirdPartyRecipeService> _thirdPartyServiceMock;
    private readonly Mock<ILogger<RecipeService>> _loggerMock;
    private readonly RecipeService _recipeService;

    public RecipeServiceTests()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _thirdPartyServiceMock = new Mock<IThirdPartyRecipeService>();
        _loggerMock = new Mock<ILogger<RecipeService>>();
        _recipeService = new RecipeService(_recipeRepositoryMock.Object, _thirdPartyServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetRecipesAsync_ShouldReturnRecipesFromDatabase()
    {
        // Arrange
        var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
        var recipesFromDB = new List<RecipeResponse>
        {
            new RecipeResponse { Name = "Tomato Soup" }
        };
        _recipeRepositoryMock.Setup(repo => repo.GetRecipesAsync(request)).ReturnsAsync(recipesFromDB);

        // Act
        var result = await _recipeService.GetRecipesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Tomato Soup", result.First().Name);
    }

    [Fact]
    public async Task GetRecipesAsync_ShouldReturnRecipesFromThirdPartyService_WhenEnabled()
    {
        // Arrange
        Environment.SetEnvironmentVariable(Common.EDAMAME_API_ACTIVE, "true");
        var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
        var recipesFromThirdParty = new List<RecipeResponse>
        {
            new RecipeResponse { Name = "Cheese Pizza" }
        };
        _thirdPartyServiceMock.Setup(service => service.GetRecipesAsync(request)).ReturnsAsync(recipesFromThirdParty);

        // Act
        var result = await _recipeService.GetRecipesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Cheese Pizza", result.First().Name);
    }

    [Fact]
    public async Task GetRecipesAsync_ShouldReturnCombinedRecipesFromDatabaseAndThirdPartyService_WhenEnabled()
    {
        // Arrange
        Environment.SetEnvironmentVariable(Common.EDAMAME_API_ACTIVE, "true");
        var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
        var recipesFromDB = new List<RecipeResponse>
        {
            new RecipeResponse { Name = "Tomato Soup" }
        };
        var recipesFromThirdParty = new List<RecipeResponse>
        {
            new RecipeResponse { Name = "Cheese Pizza" }
        };
        _recipeRepositoryMock.Setup(repo => repo.GetRecipesAsync(request)).ReturnsAsync(recipesFromDB);
        _thirdPartyServiceMock.Setup(service => service.GetRecipesAsync(request)).ReturnsAsync(recipesFromThirdParty);

        // Act
        var result = await _recipeService.GetRecipesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Tomato Soup");
        Assert.Contains(result, r => r.Name == "Cheese Pizza");
    }

    [Fact]
    public async Task GetRecipesAsync_ShouldReturnEmptyList_WhenNoRecipesFound()
    {
        // Arrange
        var request = new RecipeRequest { Ingredients = new List<string> { "Tomato", "Cheese" } };
        _recipeRepositoryMock.Setup(repo => repo.GetRecipesAsync(request)).ReturnsAsync((IEnumerable<RecipeResponse>)null);

        // Act
        var result = await _recipeService.GetRecipesAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}