using MongoDB.Driver;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Enums;

namespace Recipe.Infrastructure.MongoDBRepo;

public class UserCreatedRecipeRepository : IUserCreatedRecipeRepository
{
    private const string CollectionName = "user_created_recipes";
    private readonly IMongoCollection<UserCreatedRecipeDocument> _collection;

    public UserCreatedRecipeRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<UserCreatedRecipeDocument>(CollectionName);
    }

    public async Task<RecipeDetailResponse?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        var doc = await _collection.Find(d => d.Id == id).FirstOrDefaultAsync(ct);
        return doc == null ? null : ToDetailResponse(doc);
    }

    public async Task<IReadOnlyList<RecipeDetailResponse>> GetByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var cursor = await _collection
            .Find(d => d.UserId == userId)
            .SortByDescending(d => d.CreatedAt)
            .ToCursorAsync(ct);
        var list = await cursor.ToListAsync(ct);
        return list.Select(ToDetailResponse).ToList();
    }

    public async Task<string> AddAsync(string userId, CreateUserRecipeRequest request, CancellationToken ct = default)
    {
        var id = Guid.NewGuid().ToString();
        var doc = new UserCreatedRecipeDocument
        {
            Id = id,
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            Ingredients = request.Ingredients ?? [],
            Directions = request.Directions ?? [],
            TotalTime = request.TotalTime,
            Calories = request.Calories,
            CreatedAt = DateTime.UtcNow
        };
        await _collection.InsertOneAsync(doc, cancellationToken: ct);
        return id;
    }

    public async Task DeleteAsync(string userId, string recipeId, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(d => d.UserId == userId && d.Id == recipeId, ct);
    }

    private static RecipeDetailResponse ToDetailResponse(UserCreatedRecipeDocument doc)
    {
        return new RecipeDetailResponse
        {
            Id = doc.Id,
            Name = doc.Name,
            Images = [],
            Ingredients = doc.Ingredients.Select(t => new Ingredient { Text = t }).ToList(),
            MissingIngredients = [],
            Calories = doc.Calories,
            TotalTime = doc.TotalTime,
            CuisinTypes = [],
            MealTypes = [],
            Directions = doc.Directions.Select(t => new Direction { InstructionText = t }).ToList(),
            RecipeSourceType = RecipeSourceType.UserCreated
        };
    }
}
