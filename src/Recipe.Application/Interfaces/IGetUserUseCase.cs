using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IGetUserUseCase
{
    Task<UserResponse?> ExecuteAsync(string userId, CancellationToken ct = default);
}
