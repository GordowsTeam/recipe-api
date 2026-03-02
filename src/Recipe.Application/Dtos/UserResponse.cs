namespace Recipe.Application.Dtos;

public class UserResponse
{
    public required string Id { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
