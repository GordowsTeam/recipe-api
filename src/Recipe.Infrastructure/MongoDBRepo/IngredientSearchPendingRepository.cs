using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Core.Enums;
using Recipe.Core.Models;

namespace Recipe.Infrastructure.MongoDBRepo
{
    public class IngredientSearchPendingRepository : IIngredientSearchPendingRepository
    {
        private readonly IMongoCollection<IngredientSearchPending> _collection;

        public IngredientSearchPendingRepository(IMongoDatabase mongoDatabase)
        {
            _collection = mongoDatabase.GetCollection<IngredientSearchPending>("ingredientSearchPending");
        }
        public async Task AddAsync(IngredientSearchPending ingredientSearchPending)
        {
            await _collection.InsertOneAsync(ingredientSearchPending);
        }

        public async Task<List<IngredientSearchPending>> GetUnprocessedAsync(int limit = 50)
        {
            var filter = Builders<IngredientSearchPending>.Filter.Eq(x => x.Status, IngredientSearchStatus.Unprocessed) &
                         Builders<IngredientSearchPending>.Filter.Eq(x => x.Error, null);

            return await _collection.Find(filter)
                                    .SortBy(x => x.CreatedAt)
                                    .Limit(limit)
                                    .ToListAsync();
        }

        public async Task MarkCompletedAsync(Guid id)
        {
            var filter = Builders<IngredientSearchPending>.Filter.Eq(x => x.Id, id);
            var update = Builders<IngredientSearchPending>.Update.Set(x => x.Status, IngredientSearchStatus.Completed).Set(x => x.ProcessedAt, DateTime.UtcNow);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task MarkFailedAsync(Guid id, string error)
        {
            var filter = Builders<IngredientSearchPending>.Filter.Eq(x => x.Id, id);
            var update = Builders<IngredientSearchPending>.Update
                .Set(x => x.Status, IngredientSearchStatus.Failed)
                .Set(x => x.Error, error)
                .Set(x => x.ProcessedAt, DateTime.UtcNow)
                .Inc(x => x.RetryCount, 1);
            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task MarkProcessingAsync(Guid id)
        {
            var filter = Builders<IngredientSearchPending>.Filter.Eq(x => x.Id, id);
            var update = Builders<IngredientSearchPending>.Update.Set(x => x.Status, IngredientSearchStatus.Processing);
            await _collection.UpdateOneAsync(filter, update);
        }
    }
}
