using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Infrastructure.MongoDBRepo;

public class UserRoleRepository : IUserRoleRepository
{
    private const string CollectionName = "UserRoles";
    private readonly IMongoCollection<UserRole> _collection;

    public UserRoleRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserRole>(CollectionName);
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection.Find(r => r.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<UserRole>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
            return [];

        var filter = Builders<UserRole>.Filter.In(r => r.Id, idList);
        var cursor = await _collection.Find(filter).ToCursorAsync(ct);
        return await cursor.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken ct = default)
    {
        var cursor = await _collection.Find(FilterDefinition<UserRole>.Empty).ToCursorAsync(ct);
        return await cursor.ToListAsync(ct);
    }

    public async Task<UserRole> CreateAsync(UserRole userRole, CancellationToken ct = default)
    {
        await _collection.InsertOneAsync(userRole, cancellationToken: ct);
        return userRole;
    }
}
