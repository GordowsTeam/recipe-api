using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Application.Dtos;
using Recipe.Domain.Enums;
using Xunit;

namespace Recipe.Application.Tests.Services;

public class GetRecipeUseCaseTests
{
    private readonly Mock<IRecipeServiceFactory> _recipeServiceFactoryMock;
    private readonly Mock<ILogger<GetRecipeUseCase>> _loggerMock;
    private readonly GetRecipeUseCase _useCase;

    public GetRecipeUseCaseTests()
    {
        _recipeServiceFactoryMock = new Mock<IRecipeServiceFactory>();
        _loggerMock = new Mock<ILogger<GetRecipeUseCase>>();
        _useCase = new GetRecipeUseCase(_recipeServiceFactoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRecipeId_ReturnsRecipeDetail()
    {
        // Arrange
        var recipeId = 1;
        var recipeSourceType = RecipeSourceType.None;
        var expectedRecipe = new RecipeDetailResponse
        {
            Id = recipeId.ToString(),
            Name = "Test Recipe"
        };

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipeByIdAsync(recipeId.ToString(), Core.Enums.Language.Spanish))
            .ReturnsAsync(expectedRecipe);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act
        var result = await _useCase.ExecuteAsync(recipeId.ToString(), recipeSourceType, Core.Enums.Language.Spanish);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedRecipe.Id, result.Id);
        Assert.Equal(expectedRecipe.Name, result.Name);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipeByIdAsync(recipeId.ToString(), Core.Enums.Language.Spanish), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRecipeId_ThrowsArgumentException()
    {
        // Arrange
        string? recipeId = null;
        var recipeSourceType = RecipeSourceType.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(recipeId!, recipeSourceType, Core.Enums.Language.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyRecipeId_ThrowsArgumentException()
    {
        // Arrange
        var recipeId = string.Empty;
        var recipeSourceType = RecipeSourceType.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _useCase.ExecuteAsync(recipeId, recipeSourceType, Core.Enums.Language.None));
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceReturnsNull_ReturnsNull()
    {
        // Arrange
        var recipeId = "test-recipe-id";
        var recipeSourceType = RecipeSourceType.None;

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipeByIdAsync(recipeId, Core.Enums.Language.Spanish))
            .ReturnsAsync((RecipeDetailResponse?)null);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act
        var result = await _useCase.ExecuteAsync(recipeId, recipeSourceType, Core.Enums.Language.Spanish);

        // Assert
        Assert.Null(result);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipeByIdAsync(recipeId, Core.Enums.Language.Spanish), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenServiceThrowsException_PropagatesException()
    {
        // Arrange
        var recipeId = "test-recipe-id";
        var recipeSourceType = RecipeSourceType.None;
        var expectedException = new Exception("Service error");

        var recipeServiceMock = new Mock<IRecipeService>();
        recipeServiceMock
            .Setup(x => x.GetRecipeByIdAsync(recipeId, Core.Enums.Language.Spanish))
            .ThrowsAsync(expectedException);

        _recipeServiceFactoryMock
            .Setup(x => x.CreateRecipeService(recipeSourceType))
            .Returns(recipeServiceMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            _useCase.ExecuteAsync(recipeId, recipeSourceType, Core.Enums.Language.Spanish));
        Assert.Equal(expectedException.Message, exception.Message);
        _recipeServiceFactoryMock.Verify(x => x.CreateRecipeService(recipeSourceType), Times.Once);
        recipeServiceMock.Verify(x => x.GetRecipeByIdAsync(recipeId, Core.Enums.Language.Spanish), Times.Once);
    }
}