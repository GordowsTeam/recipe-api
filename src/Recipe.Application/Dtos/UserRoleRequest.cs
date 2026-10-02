namespace Recipe.Application.Dtos;

public class UserRoleRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
