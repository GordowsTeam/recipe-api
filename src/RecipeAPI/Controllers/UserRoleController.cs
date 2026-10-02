using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserRoleController(
    IUserRoleCatalogService userRoleCatalogService,
    ILogger<UserRoleController> logger) : ControllerBase
{
    private readonly IUserRoleCatalogService _userRoleCatalogService = userRoleCatalogService;
    private readonly ILogger<UserRoleController> _logger = logger;

    /// <summary>List all available user roles.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserRoleResponse>>> GetAll(CancellationToken ct)
    {
        try
        {
            var userRoles = await _userRoleCatalogService.GetAllAsync(ct);
            return Ok(userRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user roles.");
            return StatusCode(500, "An error occurred while loading user roles.");
        }
    }

    /// <summary>Get a single user role by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserRoleResponse>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var userRole = await _userRoleCatalogService.GetByIdAsync(id, ct);
            if (userRole == null)
                return NotFound();
            return Ok(userRole);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user role {UserRoleId}.", id);
            return StatusCode(500, "An error occurred while loading the user role.");
        }
    }

    /// <summary>Create a new user role.</summary>
    [HttpPost]
    public async Task<ActionResult<UserRoleResponse>> Create([FromBody] UserRoleRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest("Name is required.");

        try
        {
            var created = await _userRoleCatalogService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user role.");
            return StatusCode(500, "An error occurred while creating the user role.");
        }
    }
}
