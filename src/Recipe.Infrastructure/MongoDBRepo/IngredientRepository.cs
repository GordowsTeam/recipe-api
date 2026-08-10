using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models.Inventory;

namespace Recipe.Infrastructure.MongoDBRepo;

public class IngredientRepository : IIngredientRepository
{
    private const string CollectionName = "Ingredients";
    private readonly IMongoCollection<Ingredient> _collection;

    public IngredientRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Ingredient>(CollectionName);
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection.Find(i => i.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken ct = default)
    {
        var cursor = await _collection.Find(FilterDefinition<Ingredient>.Empty).ToCursorAsync(ct);
        return await cursor.ToListAsync(ct);
    }

    public async Task<Ingredient> CreateAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(ingredient, cancellationToken: ct);
        return ingredient;
    }

    public async Task<Ingredient?> UpdateAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        var result = await _collection.ReplaceOneAsync(i => i.Id == ingredient.Id, ingredient, cancellationToken: ct);
        return result.MatchedCount == 0 ? null : ingredient;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var result = await _collection.DeleteOneAsync(i => i.Id == id, ct);
        return result.DeletedCount > 0;
    }
}
