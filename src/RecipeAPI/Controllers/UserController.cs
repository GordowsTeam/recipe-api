using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(
    ICurrentUserService currentUserService,
    IGetUserUseCase getUserUseCase,
    IUpsertUserUseCase upsertUserUseCase,
    IAddFavoriteRecipeUseCase addFavoriteRecipeUseCase,
    ILogger<UserController> logger) : ControllerBase
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IGetUserUseCase _getUserUseCase = getUserUseCase;
    private readonly IUpsertUserUseCase _upsertUserUseCase = upsertUserUseCase;
    private readonly IAddFavoriteRecipeUseCase _addFavoriteRecipeUseCase = addFavoriteRecipeUseCase;
    private readonly ILogger<UserController> _logger = logger;

    /// <summary>Get the current user's profile (requires X-User-Id header or JWT).</summary>
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetMe(CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");

        try
        {
            var user = await _getUserUseCase.ExecuteAsync(userId, ct);
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

    /// <summary>Create or update the current user's profile.</summary>
    [HttpPut("me")]
    public async Task<ActionResult<UserResponse>> PutMe([FromBody] CreateOrUpdateUserRequest request, CancellationToken ct)
    {
        var userId = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");

        try
        {
            var user = await _upsertUserUseCase.ExecuteAsync(userId, request ?? new CreateOrUpdateUserRequest(), ct);
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
            return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding favorite.");
            return StatusCode(500, "An error occurred while adding the favorite.");
        }
    }
}
