using Recipe.Application.Dtos;
namespace Recipe.Application.Interfaces
{
    public interface IRecipeRepository
    {
        public Task InsertRecipeAsync(Domain.Models.Recipe recipe);
        public Task<Domain.Models.Recipe?> GetRecipeByIdAsync(Guid id);
        public Task<IEnumerable<Domain.Models.Recipe>?> GetRecipesAsync(RecipeRequest recipeRequest);
    }
}
