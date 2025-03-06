using Microsoft.Extensions.Logging;
using Moq;
using Recipe.Application.Constants;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Core.Models;
using Shouldly;

namespace Recipe.UnitTests
{
    public class RecipeServiceUnitTests
    {
        private readonly Mock<IRecipeRepository> _mockRecipeRepository;
        private readonly Mock<IThirdPartyRecipeService> _mockThirdPartyService;
        private readonly Mock<ILogger<RecipeService>> _mockLogger;

        public RecipeServiceUnitTests()
        {
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _mockThirdPartyService = new Mock<IThirdPartyRecipeService>();
            _mockLogger = new Mock<ILogger<RecipeService>>();
        }

        [Fact]
        public async Task GetRecipes_ShouldBeOfTypeCorrect()
        {
            //Arrange
            Environment.SetEnvironmentVariable(Common.EDAMAME_API_ACTIVE, "true");
            var recipeService = new RecipeService(_mockRecipeRepository.Object, _mockThirdPartyService.Object, _mockLogger.Object);
            var request = new RecipeRequest();

            //Act
            var recipes = await recipeService.GetRecipesAsync(request);

            //Asserts
            recipes.ShouldBeOfType<List<RecipeResponse>>();
            _mockRecipeRepository.Verify(v => v.GetRecipesAsync(request), Times.Once);
            _mockThirdPartyService.Verify(v => v.GetRecipesAsync(request), Times.Once);
        }
    }
}
