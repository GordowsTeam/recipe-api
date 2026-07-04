using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;

namespace Recipe.Infrastructure.MongoDBRepo;

public class UserRecipeRepository : IUserRecipeRepository
{
    private const string CollectionName = "user_recipes";
    private readonly IMongoCollection<UserRecipeDocument> _collection;

    public UserRecipeRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserRecipeDocument>(CollectionName);
    }

    public async Task<IReadOnlyList<UserRecipeRef>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var cursor = await _collection
            .Find(d => d.UserId == userId)
            .SortByDescending(d => d.AddedAt)
            .ToCursorAsync(ct);
        var list = await cursor.ToListAsync(ct);
        return list
            .Select(d => new UserRecipeRef(d.RecipeId, d.RecipeSourceType))
            .ToList();
    }

    public async Task AddAsync(string userId, string recipeId, RecipeSourceType recipeSourceType, CancellationToken ct = default)
    {
        var doc = new UserRecipeDocument
        {
            Id = $"{userId}_{recipeId}",
            UserId = userId,
            RecipeId = recipeId,
            RecipeSourceType = recipeSourceType,
            AddedAt = DateTime.UtcNow
        };
        await _collection.ReplaceOneAsync(
            d => d.Id == doc.Id,
            doc,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task RemoveAsync(string userId, string recipeId, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(d => d.UserId == userId && d.RecipeId == recipeId, ct);
    }
}
