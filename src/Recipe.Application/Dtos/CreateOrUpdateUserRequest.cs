namespace Recipe.Application.Dtos;

public class CreateOrUpdateUserRequest
{
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public List<Guid>? UserRoleIds { get; set; }
}
