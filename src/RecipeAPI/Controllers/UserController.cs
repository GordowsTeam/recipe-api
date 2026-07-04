using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    ICurrentUserService currentUserService,
    IGetUserUseCase getUserUseCase,
    IGetOrCreateUserUseCase getOrCreateUserUseCase,
    IUpsertUserUseCase upsertUserUseCase,
    IAddFavoriteRecipeUseCase addFavoriteRecipeUseCase,
    ILogger<UserController> logger) : ControllerBase
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IGetUserUseCase _getUserUseCase = getUserUseCase;
    private readonly IGetOrCreateUserUseCase _getOrCreateUserUseCase = getOrCreateUserUseCase;
    private readonly IUpsertUserUseCase _upsertUserUseCase = upsertUserUseCase;
    private readonly IAddFavoriteRecipeUseCase _addFavoriteRecipeUseCase = addFavoriteRecipeUseCase;
    private readonly ILogger<UserController> _logger = logger;

    /// <summary>Get the current user's profile (requires <c>X-User-Email</c> or JWT with <c>email</c>). Returns 404 if the user is not in the database.</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
    {
        var userEmail = _currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");

        try
        {
            var user = await _getUserUseCase.ExecuteAsync(userEmail, ct);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user.");
            return StatusCode(500, "An error occurred while loading the user.");
        }
    }

    /// <summary>
    /// Get the current user's profile; if they do not exist yet, creates them using <c>cognito:username</c> (display name)
    /// and email as the user key. Identity is JWT <c>email</c> or <c>X-User-Email</c>; optional <c>X-Cognito-Username</c> when testing without JWT.
    /// </summary>
    [HttpGet("info")]
    public async Task<ActionResult<UserResponse>> GetInfo(CancellationToken ct)
    {
        var userEmail = _currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");

        try
        {
            var user = await _getOrCreateUserUseCase.ExecuteAsync(
                userEmail,
                _currentUserService.GetCognitoUsername(),
                ct);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading or creating user.");
            return StatusCode(500, "An error occurred while loading the user.");
        }
    }

    /// <summary>Create or update the current user's profile.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> PutMe([FromBody] CreateOrUpdateUserRequest request, CancellationToken ct)
    {
        var userEmail = _currentUserService.GetUserEmail();
        if (string.IsNullOrEmpty(userEmail))
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");

        try
        {
            var user = await _upsertUserUseCase.ExecuteAsync(userEmail, request ?? new CreateOrUpdateUserRequest(), ct);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error upserting user.");
            return StatusCode(500, "An error occurred while saving the user.");
        }
    }

    /// <summary>Add a recipe to the current user's favorites.</summary>
    [HttpPost("favorites")]
    public async Task<ActionResult> AddFavorite([FromBody] AddMyRecipeRequest request, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(request?.RecipeId))
            return BadRequest("RecipeId is required.");

        try
        {
            await _addFavoriteRecipeUseCase.ExecuteAsync(request.RecipeId, request.RecipeSourceType, ct);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite.");
            return StatusCode(500, "An error occurred while adding the favorite.");
        }
    }
}
