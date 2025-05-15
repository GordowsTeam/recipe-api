using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;
using Recipe.Core.Enums;
using Xunit;

namespace Recipe.Application.Tests.Services;

public class RecipeSearchUseCaseTests
{
    private readonly Mock<IRecipeServiceFactory> _recipeServiceFactoryMock;
    private readonly Mock<ILogger<RecipeSearchUseCase>> _loggerMock;
    private readonly RecipeSearchUseCase _useCase;

    public RecipeSearchUseCaseTests()
    {
        _recipeServiceFactoryMock = new Mock<IRecipeServiceFactory>();
        _loggerMock = new Mock<ILogger<RecipeSearchUseCase>>();
        _useCase = new RecipeSearchUseCase(_recipeServiceFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsRecipes()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = new List<string> { "chicken", "rice" }
        };
        var recipeSourceType = RecipeSourceType.Edamam;
        var expectedRecipes = new List<RecipeListResponse>
        {
            new() { Id = "1", Name = "Chicken Rice Bowl" },
            new() { Id = "2", Name = "Chicken Fried Rice" }
        };

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipesAsync(recipeRequest))
            .ReturnsAsync(expectedRecipes);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act
        var result = await _useCase.ExecuteAsync(recipeRequest, recipeSourceType);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRecipes.Count, result.Count());
        Assert.Equal(expectedRecipes[0].Id, result.First().Id);
        Assert.Equal(expectedRecipes[0].Name, result.First().Name);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipesAsync(recipeRequest), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        RecipeRequest? recipeRequest = null;
        var recipeSourceType = RecipeSourceType.Edamam;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _useCase.ExecuteAsync(recipeRequest!, recipeSourceType));
    }

    [Fact]
    public async Task ExecuteAsync_WithNullIngredients_ThrowsArgumentException()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = null
        };
        var recipeSourceType = RecipeSourceType.Edamam;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _useCase.ExecuteAsync(recipeRequest, recipeSourceType));
        Assert.Equal("Ingredients cannot be null or empty (Parameter 'recipeRequest.Ingredients')", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyIngredients_ThrowsArgumentException()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = new List<string>()
        };
        var recipeSourceType = RecipeSourceType.Edamam;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _useCase.ExecuteAsync(recipeRequest, recipeSourceType));
        Assert.Equal("Ingredients cannot be null or empty (Parameter 'recipeRequest.Ingredients')", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyIngredient_ThrowsArgumentException()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = new List<string> { "chicken", "" }
        };
        var recipeSourceType = RecipeSourceType.Edamam;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
            _useCase.ExecuteAsync(recipeRequest, recipeSourceType));
        Assert.Equal("Ingredients cannot be null or empty (Parameter 'recipeRequest.Ingredients')", exception.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceReturnsNull_ReturnsNull()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = new List<string> { "chicken", "rice" }
        };
        var recipeSourceType = RecipeSourceType.Edamam;

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipesAsync(recipeRequest))
            .ReturnsAsync((IEnumerable<RecipeListResponse>?)null);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act
        var result = await _useCase.ExecuteAsync(recipeRequest, recipeSourceType);

        // Assert
        Assert.Null(result);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipesAsync(recipeRequest), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        var recipeRequest = new RecipeRequest
        {
            Ingredients = new List<string> { "chicken", "rice" }
        };
        var recipeSourceType = RecipeSourceType.Edamam;
        var expectedException = new Exception("Service error");

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipesAsync(recipeRequest))
            .ThrowsAsync(expectedException);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            _useCase.ExecuteAsync(recipeRequest, recipeSourceType));
        Assert.Equal(expectedException.Message, exception.Message);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipesAsync(recipeRequest), Times.Once);
    }
} 