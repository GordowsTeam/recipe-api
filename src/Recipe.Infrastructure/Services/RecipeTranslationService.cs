using Recipe.Application.Helpers;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Recipe.Core.Models;
using System.Text.Json;

namespace Recipe.Infrastructure.Services
{
    public class RecipeTranslationService: IRecipeTranslationService
    {
        private readonly IChatModel _chatModel;

        public RecipeTranslationService(IChatModel chatModel)
        {
            _chatModel = chatModel;
        }

        public async Task<RecipeTranslation?> TranslateRecipeAsync(Domain.Models.Recipe recipe, Language targetLanguage)
        {
            var recipeTranslationTemplate = JsonSchemaHelper.GetJsonTemplate<RecipeTranslation>();
            var systemPrompt = $"You are a translator. Translate this recipe into {targetLanguage} and ALWAYS RETURN valid JSON ONLY in the SAME STRUCTURE as this template:: {recipeTranslationTemplate}.";

            var userPrompt = JsonSerializer.Serialize(recipe);

            var resultText = await _chatModel.GetChatCompletionAsync(systemPrompt, userPrompt);

            if (string.IsNullOrWhiteSpace(resultText))
                return null;

            try
            {
                var recipeTranslation = DeserializerHelper.DeserializeSafe<RecipeTranslation>(resultText);

                return recipeTranslation;
            }
            catch
            {
                // Log deserialization error
                return null;
            }
        }
    }
}
