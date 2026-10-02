using Recipe.Application.Dtos;
using Recipe.Application.Helpers;
using Recipe.Application.Interfaces;

namespace Recipe.Application.Services;

public class GetUserUseCase(IUserRepository userRepository, IUserRoleRepository userRoleRepository) : IGetUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;

    public async Task<UserResponse?> ExecuteAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user == null)
            return null;

        var userRoles = await _userRoleRepository.GetByIdsAsync(user.UserRoleIds, ct);
        return UserResponseMapper.ToResponse(user, userRoles);
    }
}
