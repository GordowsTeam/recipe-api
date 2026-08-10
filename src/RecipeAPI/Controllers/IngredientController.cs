using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;

namespace RecipeAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngredientController(
    IIngredientCatalogService ingredientCatalogService,
    ILogger<IngredientController> logger) : ControllerBase
{
    private readonly IIngredientCatalogService _ingredientCatalogService = ingredientCatalogService;
    private readonly ILogger<IngredientController> _logger = logger;

    /// <summary>List all ingredients in the catalog.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<IngredientResponse>>> GetAll(CancellationToken ct)
    {
        try
        {
            var ingredients = await _ingredientCatalogService.GetAllAsync(ct);
            return Ok(ingredients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading ingredients.");
            return StatusCode(500, "An error occurred while loading ingredients.");
        }
    }

    /// <summary>Get a single ingredient by id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IngredientResponse>> GetById(Guid id, CancellationToken ct)
    {
        try
        {
            var ingredient = await _ingredientCatalogService.GetByIdAsync(id, ct);
            if (ingredient == null)
                return NotFound();
            return Ok(ingredient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading ingredient {IngredientId}.", id);
            return StatusCode(500, "An error occurred while loading the ingredient.");
        }
    }

    /// <summary>Create a new ingredient in the catalog.</summary>
    [HttpPost]
    public async Task<ActionResult<IngredientResponse>> Create([FromBody] IngredientRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest("Name is required.");

        try
        {
            var created = await _ingredientCatalogService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ingredient.");
            return StatusCode(500, "An error occurred while creating the ingredient.");
        }
    }

    /// <summary>Update an existing ingredient.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IngredientResponse>> Update(Guid id, [FromBody] IngredientRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest("Name is required.");

        try
        {
            var updated = await _ingredientCatalogService.UpdateAsync(id, request, ct);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ingredient {IngredientId}.", id);
            return StatusCode(500, "An error occurred while updating the ingredient.");
        }
    }

    /// <summary>Delete an ingredient from the catalog.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            var deleted = await _ingredientCatalogService.DeleteAsync(id, ct);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting ingredient {IngredientId}.", id);
            return StatusCode(500, "An error occurred while deleting the ingredient.");
        }
    }
}
