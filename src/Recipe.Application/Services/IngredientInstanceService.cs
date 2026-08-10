using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models.Inventory;

namespace Recipe.Application.Services;

public class IngredientInstanceService(
    IIngredientInstanceRepository ingredientInstanceRepository,
    ICurrentUserService currentUserService) : IIngredientInstanceService
{
    private readonly IIngredientInstanceRepository _ingredientInstanceRepository = ingredientInstanceRepository;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<IngredientInstanceResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var instance = await _ingredientInstanceRepository.GetByIdAsync(id, ct);
        return instance == null ? null : ToResponse(instance);
    }

    public async Task<IReadOnlyList<IngredientInstanceResponse>> GetMineAsync(CancellationToken ct = default)
    {
        var userId = RequireUserId();
        var instances = await _ingredientInstanceRepository.GetByUserIdAsync(userId, ct);
        return instances.Select(ToResponse).ToList();
    }

    public async Task<IngredientInstanceResponse> CreateAsync(IngredientInstanceRequest request, CancellationToken ct = default)
    {
        var userId = RequireUserId();
        var instance = new IngredientInstance
        {
            Id = Guid.NewGuid(),
            IngredientId = request.IngredientId,
            UserId = userId,
            Status = request.Status,
            Quantity = request.Quantity,
            Unit = request.Unit,
            PurchasedAt = request.PurchasedAt,
            FrozenAt = request.FrozenAt,
            DefrostStartedAt = request.DefrostStartedAt,
            LastVerifiedAt = request.LastVerifiedAt,
            Notes = request.Notes
        };
        var created = await _ingredientInstanceRepository.CreateAsync(instance, ct);
        return ToResponse(created);
    }

    public async Task<IngredientInstanceResponse?> UpdateAsync(Guid id, IngredientInstanceRequest request, CancellationToken ct = default)
    {
        var existing = await _ingredientInstanceRepository.GetByIdAsync(id, ct);
        if (existing == null)
            return null;

        existing.IngredientId = request.IngredientId;
        existing.Status = request.Status;
        existing.Quantity = request.Quantity;
        existing.Unit = request.Unit;
        existing.PurchasedAt = request.PurchasedAt;
        existing.FrozenAt = request.FrozenAt;
        existing.DefrostStartedAt = request.DefrostStartedAt;
        existing.LastVerifiedAt = request.LastVerifiedAt;
        existing.Notes = request.Notes;

        var updated = await _ingredientInstanceRepository.UpdateAsync(existing, ct);
        return updated == null ? null : ToResponse(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) => _ingredientInstanceRepository.DeleteAsync(id, ct);

    private string RequireUserId()
    {
        var userEmail = _currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            throw new UnauthorizedAccessException("User email is required.");
        return userEmail;
    }

    private static IngredientInstanceResponse ToResponse(IngredientInstance instance)
    {
        return new IngredientInstanceResponse
        {
            Id = instance.Id,
            IngredientId = instance.IngredientId,
            Status = instance.Status,
            Quantity = instance.Quantity,
            Unit = instance.Unit,
            PurchasedAt = instance.PurchasedAt,
            FrozenAt = instance.FrozenAt,
            DefrostStartedAt = instance.DefrostStartedAt,
            LastVerifiedAt = instance.LastVerifiedAt,
            Notes = instance.Notes
        };
    }
}
