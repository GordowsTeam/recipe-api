using Recipe.Domain.Models;

namespace Recipe.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task<User> UpsertAsync(User user, CancellationToken ct = default);
}
