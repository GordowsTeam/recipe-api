using Recipe.Application.Interfaces;
using OpenAI.Chat;
using System.Text.Json;
using Recipe.Application.Helpers;

namespace Recipe.Infrastructure.Services
{
    public class AIEnricher : IAIEnricher
    {
        private readonly IChatModel _chatModel;

        public AIEnricher(IChatModel chatModel)
        {
            _chatModel = chatModel;
        }
        public async Task<Domain.Models.Recipe> EnrichRecipeAsync(Domain.Models.Recipe recipe)
        {
            ArgumentNullException.ThrowIfNull(recipe);

            var recipeTemplate = JsonSchemaHelper.GetJsonTemplate<Domain.Models.Recipe>();
            var rawRecipe = JsonSerializer.Serialize(recipe, new JsonSerializerOptions { WriteIndented = true });
            var systemPrompt = "You are a professional chef and culinary assistant that enriches recipe data.";
            var userPrompt = $@"
Enrich the following recipe data by:
- Adding missing ingredients (as plain names)
- Adding missing directions or steps
- Suggesting an image URL if missing
- Suggesting cuisine and meal types if not provided
- Estimating calories and total time if unknown

- ALWAYS RETURN valid JSON ONLY in the SAME STRUCTURE as this template:
{recipeTemplate}

Recipe to enrich: {rawRecipe}";

            var resultText = await _chatModel.GetChatCompletionAsync(systemPrompt, userPrompt);

            if(string.IsNullOrWhiteSpace(resultText))
                return recipe;

            try
            {
                var enriched = DeserializerHelper.DeserializeSafe<Domain.Models.Recipe>(resultText);

                return MergeRecipes(recipe, enriched!);
            }
            catch 
            {
                // Log deserialization error
                return recipe;
            }
        }

        private static Domain.Models.Recipe MergeRecipes(Domain.Models.Recipe original, Domain.Models.Recipe enriched)
        {
            if(enriched == null)
                return original;

            if (string.IsNullOrWhiteSpace(original.Name) && !string.IsNullOrWhiteSpace(enriched.Name))
                original.Name = enriched.Name;

            original.Images ??= enriched.Images;
            original.Ingredients ??= enriched.Ingredients;
            original.Directions ??= enriched.Directions;
            original.CuisinTypes ??= enriched.CuisinTypes;
            original.MealTypes ??= enriched.MealTypes;

            if (original.Calories == 0)
                original.Calories = enriched.Calories;

            if (original.TotalTime == 0)
                original.TotalTime = enriched.TotalTime;

            original.AIEnriched = true;

            return original;
        }
    }
}
