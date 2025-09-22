using MongoDB.Driver;
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

        public async Task<IEnumerable<Domain.Models.Recipe>?> GetRecipesAsync(IEnumerable<string> ingredients)
        {
            var filterBuilder = Builders<Domain.Models.Recipe>.Filter;
            var filter = filterBuilder.Empty;

            if (ingredients != null && ingredients.Any())
            {
                // Filter recipes that have at least one ingredient matching the requested names
                filter &= filterBuilder.In("Ingredients.Name", ingredients);
            }

            var recipes = await _collection.Find(filter).ToListAsync();
            var distinctRecipes = recipes.GroupBy(r => r.Id).Select(g => g.First()).ToList();
            
            return distinctRecipes;
        }

        public async Task InsertRecipeAsync(Domain.Models.Recipe recipe) => await _collection.InsertOneAsync(recipe);
    }
}
