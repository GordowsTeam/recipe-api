using Recipe.Application.Interfaces;

namespace Recipe.Infrastructure.Services
{
    public class OpenAIEnricher : IAIEnricher
    {
        public Task<Domain.Models.Recipe> EnrichRecipeAsync(Domain.Models.Recipe recipe)
        {
            // Call OpenAI/Azure OpenAI API here
            recipe.Name = $"AI enriched description for {recipe.Name}";
            return Task.FromResult(recipe);
        }
    }
}
