using Moq;
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

        public RecipeServiceUnitTests()
        {
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _mockThirdPartyService = new Mock<IThirdPartyRecipeService>();
        }

        [Fact]
        public async Task GetRecipes_ShouldBeOfTypeCorrect()
        {
            //Arrange
            var recipeService = new RecipeService(_mockRecipeRepository.Object, _mockThirdPartyService.Object);

            //Act
            var recipes = await recipeService.GetRecipesAsync(new RecipeRequest());

            //Asserts
            recipes.ShouldBeOfType<RecipeResponse[]>();
            _mockRecipeRepository.Verify(v => v.GetRecipesAsync(new RecipeRequest()), Times.Once);
        }
    }
}
