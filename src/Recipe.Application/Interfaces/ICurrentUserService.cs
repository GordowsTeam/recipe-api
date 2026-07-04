namespace Recipe.Application.Interfaces;

public interface ICurrentUserService
{
    /// <summary>Stable user key: JWT <c>email</c> claim or <c>X-User-Email</c> header.</summary>
    string? GetUserEmail();

    /// <summary>Cognito username from JWT <c>cognito:username</c> claim or <c>X-Cognito-Username</c> header.</summary>
    string? GetCognitoUsername();
}
