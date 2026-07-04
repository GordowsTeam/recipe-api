namespace Recipe.Domain.Models;

public class User
{
    /// <summary>User id (email — stable key from Cognito <c>email</c> claim).</summary>
    public string Id { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? DisplayName { get; set; }

    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
