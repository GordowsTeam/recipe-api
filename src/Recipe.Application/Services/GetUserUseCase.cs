using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class GetUserUseCase(IUserRepository userRepository) : IGetUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResponse?> ExecuteAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        return user == null ? null : ToResponse(user);
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedDateTime = user.CreatedDateTime,
            UpdatedDateTime = user.UpdatedDateTime
        };
    }
}
