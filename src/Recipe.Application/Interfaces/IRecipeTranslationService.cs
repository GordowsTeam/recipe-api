using Recipe.Core.Models;

namespace Recipe.Application.Interfaces
{
    public interface IRecipeTranslationService
    {
        Task<RecipeTranslation?> TranslateRecipeAsync(Domain.Models.Recipe recipe, Core.Enums.Language targetLanguage);
    }
}
