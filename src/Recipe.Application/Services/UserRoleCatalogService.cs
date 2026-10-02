using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;

namespace Recipe.Application.Services;

public class UserRoleCatalogService(IUserRoleRepository userRoleRepository) : IUserRoleCatalogService
{
    private readonly IUserRoleRepository _userRoleRepository = userRoleRepository;

    public async Task<UserRoleResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var userRole = await _userRoleRepository.GetByIdAsync(id, ct);
        return userRole == null ? null : ToResponse(userRole);
    }

    public async Task<IReadOnlyList<UserRoleResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var userRoles = await _userRoleRepository.GetAllAsync(ct);
        return userRoles.Select(ToResponse).ToList();
    }

    public async Task<UserRoleResponse> CreateAsync(UserRoleRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedDateTime = now,
            UpdatedDateTime = now
        };
        var created = await _userRoleRepository.CreateAsync(userRole, ct);
        return ToResponse(created);
    }

    private static UserRoleResponse ToResponse(UserRole userRole)
    {
        return new UserRoleResponse
        {
            Id = userRole.Id,
            Name = userRole.Name,
            Description = userRole.Description,
            CreatedDateTime = userRole.CreatedDateTime,
            UpdatedDateTime = userRole.UpdatedDateTime
        };
    }
}
