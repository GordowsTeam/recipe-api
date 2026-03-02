using MongoDB.Driver;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Infrastructure.MongoDBRepo;

public class UserRepository : IUserRepository
{
    private const string CollectionName = "Users";
    private readonly IMongoCollection<UserDocument> _collection;

    public UserRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserDocument>(CollectionName);
    }

    public async Task<User?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.Id == userId).FirstOrDefaultAsync(ct);
        return doc == null ? null : ToUser(doc);
    }

    public async Task<User> UpsertAsync(User user, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var doc = ToDocument(user);
        var existing = await _collection.Find(d => d.Id == doc.Id).FirstOrDefaultAsync(ct);
        if (existing == null)
        {
            if (!doc.CreatedDateTime.HasValue)
                doc.CreatedDateTime = now;
            if (!doc.UpdatedDateTime.HasValue)
                doc.UpdatedDateTime = now;
            await _collection.InsertOneAsync(doc, cancellationToken: ct);
        }
        else
        {
            doc.CreatedDateTime = existing.CreatedDateTime;
            doc.UpdatedDateTime = now;
            await _collection.ReplaceOneAsync(d => d.Id == doc.Id, doc, new ReplaceOptions { IsUpsert = true }, ct);
        }
        return ToUser(doc);
    }

    private static UserDocument ToDocument(User user)
    {
        return new UserDocument
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedDateTime = user.CreatedDateTime,
            UpdatedDateTime = user.UpdatedDateTime
        };
    }

    private static User ToUser(UserDocument doc)
    {
        return new User
        {
            Id = doc.Id,
            Email = doc.Email,
            DisplayName = doc.DisplayName,
            CreatedDateTime = doc.CreatedDateTime,
            UpdatedDateTime = doc.UpdatedDateTime
        };
    }
}
