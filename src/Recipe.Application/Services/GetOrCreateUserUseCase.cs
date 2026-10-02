using Recipe.Application.Dtos;
using Recipe.Application.Helpers;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class GetOrCreateUserUseCase(IUserRepository userRepository, IUserRoleRepository userRoleRepository) : IGetOrCreateUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;

    public async Task<UserResponse> ExecuteAsync(string userEmail, string? cognitoUsername, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(userEmail, ct);
        var user = existing;
        if (user == null)
        {
            user = new User
            {
                Id = userEmail,
                Email = userEmail,
                DisplayName = cognitoUsername,
            };
            user = await _userRepository.UpsertAsync(user, ct);
        }

        var userRoles = await _userRoleRepository.GetByIdsAsync(user.UserRoleIds, ct);
        return UserResponseMapper.ToResponse(user, userRoles);
    }
}
