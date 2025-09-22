namespace Recipe.Application.Interfaces
{
    public interface IAIEnricher
    {
        Task<Domain.Models.Recipe> EnrichRecipeAsync(Domain.Models.Recipe recipe);
    }
}
