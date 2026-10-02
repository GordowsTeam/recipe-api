using Recipe.Application.Dtos;
using Recipe.Application.Helpers;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class UpsertUserUseCase(IUserRepository userRepository, IUserRoleRepository userRoleRepository) : IUpsertUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;

    public async Task<UserResponse> ExecuteAsync(string userId, CreateOrUpdateUserRequest request, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(userId, ct);
        var user = new User
        {
            Id = userId,
            Email = request.Email ?? existing?.Email,
            DisplayName = request.DisplayName ?? existing?.DisplayName,
            UserRoleIds = request.UserRoleIds ?? existing?.UserRoleIds ?? [],
            CreatedDateTime = existing?.CreatedDateTime,
            UpdatedDateTime = existing?.UpdatedDateTime
        };
        var upserted = await _userRepository.UpsertAsync(user, ct);
        var userRoles = await _userRoleRepository.GetByIdsAsync(upserted.UserRoleIds, ct);
        return UserResponseMapper.ToResponse(upserted, userRoles);
    }
}
