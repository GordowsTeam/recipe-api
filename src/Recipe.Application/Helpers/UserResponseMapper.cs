using Recipe.Application.Dtos;
using Recipe.Domain.Models;

namespace Recipe.Application.Helpers;

public static class UserResponseMapper
{
    public static UserResponse ToResponse(User user, IEnumerable<UserRole> userRoles)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            UserRoles = userRoles.Select(r => new UserRoleResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                CreatedDateTime = r.CreatedDateTime,
                UpdatedDateTime = r.UpdatedDateTime
            }).ToList(),
            CreatedDateTime = user.CreatedDateTime,
            UpdatedDateTime = user.UpdatedDateTime
        };
    }
}
