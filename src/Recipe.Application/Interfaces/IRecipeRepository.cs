namespace Recipe.Application.Interfaces
{
    public interface IRecipeRepository
    {
        public Task InsertRecipeAsync(Domain.Models.Recipe recipe);
    }
}
