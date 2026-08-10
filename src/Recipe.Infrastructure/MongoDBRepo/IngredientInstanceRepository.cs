using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models.Inventory;

namespace Recipe.Infrastructure.MongoDBRepo;

public class IngredientInstanceRepository : IIngredientInstanceRepository
{
    private const string CollectionName = "IngredientInstances";
    private readonly IMongoCollection<IngredientInstance> _collection;

    public IngredientInstanceRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<IngredientInstance>(CollectionName);
    }

    public async Task<IngredientInstance?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection.Find(i => i.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<IngredientInstance>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var cursor = await _collection
            .Find(i => i.UserId == userId)
            .SortByDescending(i => i.PurchasedAt)
            .ToCursorAsync(ct);
        return await cursor.ToListAsync(ct);
    }

    public async Task<IngredientInstance> CreateAsync(IngredientInstance instance, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(instance, cancellationToken: ct);
        return instance;
    }

    public async Task<IngredientInstance?> UpdateAsync(IngredientInstance instance, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(i => i.Id == instance.Id, instance, cancellationToken: ct);
        return result.MatchedCount == 0 ? null : instance;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(i => i.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
