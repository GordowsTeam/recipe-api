using Moq;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Enums;
using Recipe.Core.Models;
using Recipe.Domain.Enums;
using Recipe.Infrastructure.Services;
using Xunit;
using InventoryIngredient = Recipe.Domain.Models.Inventory.Ingredient;

namespace Recipe.Infrastructure.UnitTests;

public class RecipeETLServiceTests
{
    private readonly Mock<IIngredientSearchPendingService> _ingredientSearchPendingServiceMock = new();
    private readonly Mock<IRecipeSearchUseCase> _recipeSearchUseCaseMock = new();
    private readonly Mock<IGetRecipeUseCase> _getRecipeUseCaseMock = new();
    private readonly Mock<IAIEnricher> _aiEnricherMock = new();
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock = new();
    private readonly Mock<IDuplicateFinder> _duplicateFinderMock = new();
    private readonly Mock<IRecipeTranslationService> _recipeTranslationServiceMock = new();
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock = new();
    private readonly Mock<IIngredientEnricher> _ingredientEnricherMock = new();
    private readonly RecipeETLService _service;

    public RecipeETLServiceTests()
    {
        _service = new RecipeETLService(
            _ingredientSearchPendingServiceMock.Object,
            _recipeSearchUseCaseMock.Object,
            _getRecipeUseCaseMock.Object,
            _aiEnricherMock.Object,
            _recipeRepositoryMock.Object,
            _duplicateFinderMock.Object,
            _recipeTranslationServiceMock.Object,
            _ingredientRepositoryMock.Object,
            _ingredientEnricherMock.Object);

        // Common plumbing so tests only need to set up the ingredient-linking scenario.
        _duplicateFinderMock
            .Setup(d => d.IsDuplicateAsync(It.IsAny<Domain.Models.Recipe>(), It.IsAny<Language>()))
            .ReturnsAsync(false);
        _aiEnricherMock
            .Setup(e => e.EnrichRecipeAsync(It.IsAny<Domain.Models.Recipe>()))
            .ReturnsAsync((Domain.Models.Recipe r) => r);
        _recipeTranslationServiceMock
            .Setup(t => t.TranslateRecipeAsync(It.IsAny<Domain.Models.Recipe>(), It.IsAny<Language>()))
            .ReturnsAsync((RecipeTranslation?)null);
        _ingredientRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<InventoryIngredient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryIngredient i, CancellationToken _) => i);
    }

    private void SetupPendingRecipeWithIngredients(IEnumerable<Ingredient> ingredients)
    {
        var pending = new IngredientSearchPending { Id = Guid.NewGuid(), Ingredients = new List<string> { "chicken" } };
        _ingredientSearchPendingServiceMock.Setup(s => s.GetPendingAsync(50)).ReturnsAsync(new List<IngredientSearchPending> { pending });

        var listResponse = new RecipeListResponse { Id = Guid.NewGuid().ToString(), Name = "Chicken Soup", RecipeSourceType = RecipeSourceType.Spoonacular };
        _recipeSearchUseCaseMock
            .Setup(s => s.ExecuteAsync(It.IsAny<RecipeRequest>(), RecipeSourceType.Spoonacular))
            .ReturnsAsync(new List<RecipeListResponse> { listResponse });

        var detailResponse = new RecipeDetailResponse
        {
            Id = listResponse.Id,
            Name = listResponse.Name,
            Ingredients = ingredients,
            RecipeSourceType = RecipeSourceType.Spoonacular
        };
        _getRecipeUseCaseMock
            .Setup(s => s.ExecuteAsync(listResponse.Id, RecipeSourceType.Spoonacular, Language.Spanish))
            .ReturnsAsync(detailResponse);
    }

    [Fact]
    public async Task ProcessRecipesAsync_IngredientNotInCatalog_AsksAIThenCreatesAndLinksIt()
    {
        // Arrange
        SetupPendingRecipeWithIngredients(new[]
        {
            new Ingredient { Text = "Chicken Breast", Quantity = 1, Measure = "pza" }
        });

        _ingredientRepositoryMock
            .Setup(r => r.GetByNameAsync("Chicken Breast", It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryIngredient?)null);

        _ingredientEnricherMock
            .Setup(e => e.EnrichIngredientAsync("Chicken Breast"))
            .ReturnsAsync(new InventoryIngredient
            {
                Name = "Chicken Breast",
                Description = "Lean cut of poultry breast meat.",
                FoodCategory = FoodCategory.Proteins,
                Image = "https://example.com/chicken-breast.png"
            });

        Domain.Models.Recipe? insertedRecipe = null;
        _recipeRepositoryMock
            .Setup(r => r.InsertRecipeAsync(It.IsAny<Domain.Models.Recipe>()))
            .Callback<Domain.Models.Recipe>(r => insertedRecipe = r)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessRecipesAsync(Language.Spanish);

        // Assert
        Assert.True(result);
        Assert.NotNull(insertedRecipe);
        var linkedIngredient = insertedRecipe!.Translations![Language.English].Ingredients!.Single();
        Assert.NotNull(linkedIngredient.IngredientId);

        _ingredientEnricherMock.Verify(e => e.EnrichIngredientAsync("Chicken Breast"), Times.Once);
        _ingredientRepositoryMock.Verify(
            r => r.CreateAsync(
                It.Is<InventoryIngredient>(i =>
                    i.Name == "Chicken Breast" &&
                    i.Description == "Lean cut of poultry breast meat." &&
                    i.FoodCategory == FoodCategory.Proteins &&
                    i.Image == "https://example.com/chicken-breast.png"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessRecipesAsync_AIEnrichmentUnavailable_StillCreatesIngredientWithRecipeData()
    {
        // Arrange: AI call fails/returns nothing - the catalog entry must still be created using the data already on the recipe line.
        SetupPendingRecipeWithIngredients(new[]
        {
            new Ingredient { Text = "Chicken Breast", Quantity = 1, Measure = "pza", FoodCategory = FoodCategory.Proteins, Image = "recipe-image.png" }
        });

        _ingredientRepositoryMock
            .Setup(r => r.GetByNameAsync("Chicken Breast", It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryIngredient?)null);

        _ingredientEnricherMock
            .Setup(e => e.EnrichIngredientAsync("Chicken Breast"))
            .ReturnsAsync((InventoryIngredient?)null);

        Domain.Models.Recipe? insertedRecipe = null;
        _recipeRepositoryMock
            .Setup(r => r.InsertRecipeAsync(It.IsAny<Domain.Models.Recipe>()))
            .Callback<Domain.Models.Recipe>(r => insertedRecipe = r)
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessRecipesAsync(Language.Spanish);

        // Assert
        var linkedIngredient = insertedRecipe!.Translations![Language.English].Ingredients!.Single();
        Assert.NotNull(linkedIngredient.IngredientId);
        _ingredientRepositoryMock.Verify(
            r => r.CreateAsync(
                It.Is<InventoryIngredient>(i => i.Name == "Chicken Breast" && i.FoodCategory == FoodCategory.Proteins && i.Image == "recipe-image.png"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessRecipesAsync_IngredientAlreadyInCatalog_LinksExistingEntryWithoutCallingAIOrCreating()
    {
        // Arrange
        SetupPendingRecipeWithIngredients(new[]
        {
            new Ingredient { Text = "Chicken Breast", Quantity = 1, Measure = "pza" }
        });

        var existingCatalogIngredient = new InventoryIngredient { Id = Guid.NewGuid(), Name = "Chicken Breast" };
        _ingredientRepositoryMock
            .Setup(r => r.GetByNameAsync("Chicken Breast", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCatalogIngredient);

        Domain.Models.Recipe? insertedRecipe = null;
        _recipeRepositoryMock
            .Setup(r => r.InsertRecipeAsync(It.IsAny<Domain.Models.Recipe>()))
            .Callback<Domain.Models.Recipe>(r => insertedRecipe = r)
            .Returns(Task.CompletedTask);

        // Act
        await _service.ProcessRecipesAsync(Language.Spanish);

        // Assert
        var linkedIngredient = insertedRecipe!.Translations![Language.English].Ingredients!.Single();
        Assert.Equal(existingCatalogIngredient.Id, linkedIngredient.IngredientId);
        _ingredientEnricherMock.Verify(e => e.EnrichIngredientAsync(It.IsAny<string>()), Times.Never);
        _ingredientRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<InventoryIngredient>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
