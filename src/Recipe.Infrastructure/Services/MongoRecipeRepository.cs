using MongoDB.Driver;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace Recipe.Infrastructure.Services
{
    public class MongoRecipeRepository : IRecipeRepository
    {
        private readonly IMongoCollection<Domain.Models.Recipe> _collection;

        public MongoRecipeRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Domain.Models.Recipe>("recipes");
        }

        public async Task<Domain.Models.Recipe?> GetRecipeByIdAsync(Guid id)
        {
            return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Domain.Models.Recipe>?> GetRecipesAsync(RecipeRequest recipeRequest)
        {
            var filterBuilder = Builders<Domain.Models.Recipe>.Filter;
            var filter = filterBuilder.Empty;

            if (recipeRequest.Name != null)
            {
                filter &= filterBuilder.Regex(r => r.Name, new MongoDB.Bson.BsonRegularExpression(recipeRequest.Name, "i"));
            }

            if (recipeRequest.Ingredients != null && recipeRequest.Ingredients.Any())
            {
                var regexFilters = recipeRequest.Ingredients
                           .Select(name => filterBuilder.Regex("Ingredients.Name", new MongoDB.Bson.BsonRegularExpression(name, "i")))
                           .ToList();

                filter &= recipeRequest.MatchAllIngredients
                    ? filterBuilder.And(regexFilters)  // all ingredients
                    : filterBuilder.Or(regexFilters);  // at least one ingredient
            }

            if (recipeRequest.CuisineTypes != null && recipeRequest.CuisineTypes.Any())
            {
                //filter &= filterBuilder.In(r => r.CuisinTypes, recipeFilters.CuisineTypes);
            }

            // Filter by meal types (at least one match)
            if (recipeRequest.MealTypes != null && recipeRequest.MealTypes.Any())
            {
                //filter &= filterBuilder.In(r => r.MealTypes, recipeFilters.MealTypes);
            }

            var recipes = await _collection.Find(filter).ToListAsync();
            var distinctRecipes = recipes.GroupBy(r => r.Id).Select(g => g.First()).ToList();
            
            return distinctRecipes;
        }

        public async Task InsertRecipeAsync(Domain.Models.Recipe recipe) => await _collection.InsertOneAsync(recipe);
    }
}
