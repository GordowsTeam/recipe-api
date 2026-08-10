using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngredientInstanceController(
    IIngredientInstanceService ingredientInstanceService,
    ILogger<IngredientInstanceController> logger) : ControllerBase
{
    private readonly IIngredientInstanceService _ingredientInstanceService = ingredientInstanceService;
    private readonly ILogger<IngredientInstanceController> _logger = logger;

    /// <summary>List the current user's inventory instances (requires <c>X-User-Email</c> or JWT with <c>email</c>).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IngredientInstanceResponse>>> GetMine(CancellationToken ct)
    {
        try
        {
            var instances = await _ingredientInstanceService.GetMineAsync(ct);
            return Ok(instances);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading ingredient instances.");
            return StatusCode(500, "An error occurred while loading inventory items.");
        }
    }

    /// <summary>Get a single inventory instance by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IngredientInstanceResponse>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var instance = await _ingredientInstanceService.GetByIdAsync(id, ct);
            if (instance == null)
                return NotFound();
            return Ok(instance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading ingredient instance {IngredientInstanceId}.", id);
            return StatusCode(500, "An error occurred while loading the inventory item.");
        }
    }

    /// <summary>Add a new ingredient instance to the current user's inventory.</summary>
    [HttpPost]
    public async Task<ActionResult<IngredientInstanceResponse>> Create([FromBody] IngredientInstanceRequest request, CancellationToken ct)
    {
        if (request?.IngredientId == null || request.IngredientId == Guid.Empty)
            return BadRequest("IngredientId is required.");
        if (string.IsNullOrWhiteSpace(request.Unit))
            return BadRequest("Unit is required.");

        try
        {
            var created = await _ingredientInstanceService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User email is required. Send X-User-Email header or use a token that includes an email claim.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ingredient instance.");
            return StatusCode(500, "An error occurred while creating the inventory item.");
        }
    }

    /// <summary>Update an existing inventory instance (e.g. status, quantity).</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IngredientInstanceResponse>> Update(Guid id, [FromBody] IngredientInstanceRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request?.Unit))
            return BadRequest("Unit is required.");

        try
        {
            var updated = await _ingredientInstanceService.UpdateAsync(id, request, ct);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ingredient instance {IngredientInstanceId}.", id);
            return StatusCode(500, "An error occurred while updating the inventory item.");
        }
    }

    /// <summary>Remove an inventory instance (e.g. consumed or discarded).</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            var deleted = await _ingredientInstanceService.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ingredient instance {IngredientInstanceId}.", id);
            return StatusCode(500, "An error occurred while deleting the inventory item.");
        }
    }
}
