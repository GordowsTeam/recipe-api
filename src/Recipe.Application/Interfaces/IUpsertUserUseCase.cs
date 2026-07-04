using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IUpsertUserUseCase
{
    Task<UserResponse> ExecuteAsync(string userId, CreateOrUpdateUserRequest request, CancellationToken ct = default);
}
