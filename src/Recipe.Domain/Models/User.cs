namespace Recipe.Domain.Models;

public class User
{
    /// <summary>User id (e.g. from auth provider / Cognito sub).</summary>
    public string Id { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? DisplayName { get; set; }

    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
}
