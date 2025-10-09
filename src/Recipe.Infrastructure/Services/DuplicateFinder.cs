using MongoDB.Bson;
using MongoDB.Driver;
using Recipe.Application.Interfaces;
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

        //TODO: implement a real duplicate finder
        public async Task<bool> IsDuplicateAsync(Domain.Models.Recipe recipe)
        {
            if (recipe == null) return false;

            //1. Try to find duplicity by Name
            var filterBuilder = Builders<Domain.Models.Recipe>.Filter;
            var filter = filterBuilder.Or(
                filterBuilder.Regex(r => r.Name, new BsonRegularExpression($"^{RegexEscape(recipe.Name)}$", "i")));

            var existingExact = await _recipes.Find(filter).FirstOrDefaultAsync();
            if (existingExact != null) return true;

            //2. Try to find duplicity by Ingredient-based similarity(>80%)
            if (recipe?.Ingredients == null || !recipe.Ingredients.Any())
                return false;

            var allCandidates = await _recipes.Find(_ => true)
                .Project(r => new { r.Id, r.Name, r.Ingredients }).ToListAsync();

            var ingredients = recipe.Ingredients.Where(i => !string.IsNullOrEmpty(i.Name));
            var recipeIngredients = ingredients.Select(i => i.Name.ToLowerInvariant().Trim()).ToHashSet();

            foreach (var candidate in allCandidates)
            {
                if (candidate.Ingredients == null || !candidate.Ingredients.Any())
                    continue;

                var candidateIngredientsName = candidate.Ingredients.Where(i => !string.IsNullOrEmpty(i.Name)).ToList();
                var candidateIngredients = candidateIngredientsName.Select(i => i.Name.ToLowerInvariant().Trim()).ToHashSet();

                int intersection = candidateIngredients.Intersect(recipeIngredients).Count();
                int union = candidateIngredients.Union(recipeIngredients).Count();
                double similarity = (double)intersection / union;

                if (similarity >= 80)
                    return true;
            }

            return false;
        }

        private string RegexEscape(string text) => Regex.Escape(text ?? string.Empty);
    }
}
