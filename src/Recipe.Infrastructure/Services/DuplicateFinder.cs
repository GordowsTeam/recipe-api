using MongoDB.Bson;
using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using System.Text.RegularExpressions;

namespace Recipe.Infrastructure.Services
{
    public class DuplicateFinder : IDuplicateFinder
    {
        private readonly IMongoCollection<Domain.Models.Recipe> _recipes;
        public DuplicateFinder(IMongoDatabase database)
        {
            _recipes = database.GetCollection<Domain.Models.Recipe>("Recipes");
        }

        public async Task<bool> IsDuplicateAsync(Domain.Models.Recipe recipe, Language language)
        {
            if (recipe == null) return false;
            
            Recipe.Core.Models.RecipeTranslation? translation = null;

            // 1️⃣ Try to find duplicity by Name in the specified language
            string? recipeName = null;

            if (recipe.Translations != null && recipe.Translations.TryGetValue(language, out var t))
            {
                translation = t;
                recipeName = translation.Name;
            }
            else 
            {
                return false; // If no translation in the specified language, skip name check
            }

            if (!string.IsNullOrWhiteSpace(recipeName))
            {
                var filterBuilder = Builders<Domain.Models.Recipe>.Filter;
                var filter = filterBuilder.Or(
                    filterBuilder.Regex(
                        $"Translations.{language.ToString().ToLower()}.Name",
                        new BsonRegularExpression($"^{RegexEscape(recipeName)}$", "i")
                    )
                );

                var existingExact = await _recipes.Find(filter).FirstOrDefaultAsync();
                if (existingExact != null) return true;
            }

            // 2️⃣ Try to find duplicity by Ingredient-based similarity (>80%)
            if (translation != null && (translation.Ingredients == null || !translation.Ingredients.Any()))
                return false;

            var recipeIngredients = translation!.Ingredients?
                .Where(i => !string.IsNullOrEmpty(i.Name))
                .Select(i => i.Name?.ToLowerInvariant().Trim())
                .ToHashSet();
            
            if (recipeIngredients == null)
                return false;
            
            // Load all candidates from DB with their ingredients in the same language
            var allCandidates = await _recipes.Find(_ => true)
                .Project(r => new
                {
                    r.Id,
                    Translations = r.Translations
                })
                .ToListAsync();

            foreach (var candidate in allCandidates)
            {
                if (candidate.Translations == null || !candidate.Translations.TryGetValue(language, out var candidateTranslation))
                    continue;

                if (candidateTranslation.Ingredients == null || !candidateTranslation.Ingredients.Any())
                    continue;

                var candidateIngredients = candidateTranslation.Ingredients
                    .Where(i => !string.IsNullOrEmpty(i.Name))
                    .Select(i => i.Name?.ToLowerInvariant().Trim())
                    .ToHashSet();
                
                int intersection = candidateIngredients.Intersect(recipeIngredients).Count();
                int union = candidateIngredients.Union(recipeIngredients).Count();
                double similarity = (double)intersection / union;

                if (similarity >= 0.8) // 80%
                    return true;
            }

            return false;
        }

        private string RegexEscape(string text) => Regex.Escape(text ?? string.Empty);
    }
}
