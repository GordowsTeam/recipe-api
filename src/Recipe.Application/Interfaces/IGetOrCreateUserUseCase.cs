using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IGetOrCreateUserUseCase
{
    Task<UserResponse> ExecuteAsync(string userEmail, string? cognitoUsername, CancellationToken ct = default);
}
