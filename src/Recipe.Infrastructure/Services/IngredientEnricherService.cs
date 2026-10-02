using Recipe.Application.Helpers;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models.Inventory;

namespace Recipe.Infrastructure.Services
{
    public class IngredientEnricherService : IIngredientEnricher
    {
        private readonly IChatModel _chatModel;

        public IngredientEnricherService(IChatModel chatModel)
        {
            _chatModel = chatModel;
        }

        public async Task<Ingredient?> EnrichIngredientAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var template = JsonSchemaHelper.GetJsonTemplate<Ingredient>();
            var systemPrompt = "You are a culinary and nutrition expert that provides structured catalog metadata about food ingredients.";
            var userPrompt = $@"
Provide catalog details for the ingredient ""{name}"":
- A short description (what it is and common culinary uses)
- Its FoodCategory (pick the closest match)
- An image URL only if you are confident of a reliable public one, otherwise leave it empty

ALWAYS RETURN valid JSON ONLY in the SAME STRUCTURE as this template:
{template}";

            string resultText;
            try
            {
                resultText = await _chatModel.GetChatCompletionAsync(systemPrompt, userPrompt);
            }
            catch (Exception ex)
            {
                // Ingredient enrichment is best-effort: the catalog entry is created with basic data instead.
                Console.WriteLine($"[IngredientEnricherService] Could not enrich ingredient '{name}': {ex.Message}");
                return null;
            }

            if (string.IsNullOrWhiteSpace(resultText))
                return null;

            var enriched = DeserializerHelper.DeserializeSafe<Ingredient>(resultText);
            if (enriched == null)
                return null;

            enriched.Name = name;
            return enriched;
        }
    }
}
