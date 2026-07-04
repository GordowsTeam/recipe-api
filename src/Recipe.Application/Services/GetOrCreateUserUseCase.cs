using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class GetOrCreateUserUseCase(IUserRepository userRepository) : IGetOrCreateUserUseCase
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResponse> ExecuteAsync(string userEmail, string? cognitoUsername, CancellationToken ct = default)
    {
        var existing = await _userRepository.GetByIdAsync(userEmail, ct);
        if (existing != null)
            return ToResponse(existing);

        var user = new User
        {
            Id = userEmail,
            Email = userEmail,
            DisplayName = cognitoUsername,
        };
        var created = await _userRepository.UpsertAsync(user, ct);
        return ToResponse(created);
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            CreatedDateTime = user.CreatedDateTime,
            UpdatedDateTime = user.UpdatedDateTime,
        };
    }
}
