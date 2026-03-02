using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class UpsertUserUseCase(IUserRepository userRepository) : IUpsertUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResponse> ExecuteAsync(string userId, CreateOrUpdateUserRequest request, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(userId, ct);
        var user = new User
        {
            Id = userId,
            Email = request.Email ?? existing?.Email,
            DisplayName = request.DisplayName ?? existing?.DisplayName,
            CreatedDateTime = existing?.CreatedDateTime,
            UpdatedDateTime = existing?.UpdatedDateTime
        };
        var upserted = await _userRepository.UpsertAsync(user, ct);
        return new UserResponse
        {
            Id = upserted.Id,
            Email = upserted.Email,
            DisplayName = upserted.DisplayName,
            CreatedDateTime = upserted.CreatedDateTime,
            UpdatedDateTime = upserted.UpdatedDateTime
        };
    }
}
