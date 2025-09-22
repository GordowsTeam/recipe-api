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

        public async Task InsertRecipeAsync(Domain.Models.Recipe recipe) => await _collection.InsertOneAsync(recipe);
    }
}
