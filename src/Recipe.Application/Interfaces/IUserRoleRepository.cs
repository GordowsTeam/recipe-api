using Recipe.Domain.Models;

namespace Recipe.Application.Interfaces;

public interface IUserRoleRepository
{
    Task<UserRole?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserRole>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task<IReadOnlyList<UserRole>> GetAllAsync(CancellationToken ct = default);
    Task<UserRole> CreateAsync(UserRole userRole, CancellationToken ct = default);
}
