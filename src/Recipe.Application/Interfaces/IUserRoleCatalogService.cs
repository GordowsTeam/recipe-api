using Recipe.Application.Dtos;

namespace Recipe.Application.Interfaces;

public interface IUserRoleCatalogService
{
    Task<UserRoleResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<UserRoleResponse>> GetAllAsync(CancellationToken ct = default);
    Task<UserRoleResponse> CreateAsync(UserRoleRequest request, CancellationToken ct = default);
}
