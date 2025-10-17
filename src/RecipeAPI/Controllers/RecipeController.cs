using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Interfaces;
using Recipe.Domain.Models;
using Recipe.Domain.Enums;
using Recipe.Application.Services;
using Recipe.Application.Dtos;
using Recipe.Application.Validators;
using Recipe.Core.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController(
        IRecipeSearchUseCase recipeSearchUseCase,
        IGetRecipeUseCase getRecipeUseCase,
        ILogger<RecipeController> logger) : ControllerBase
    {
        private readonly IRecipeSearchUseCase _recipeSearchUseCase = recipeSearchUseCase;
        private readonly IGetRecipeUseCase _getRecipeUseCase = getRecipeUseCase;

        private readonly ILogger<RecipeController> _logger = logger;

        // GET: api/<RecipeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return await Task.FromResult(new string[] { "Test", "value2" });
        }

        // GET api/<RecipeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeListResponse>> Get(string id, [FromQuery] RecipeSourceType? recipeSourceType = null, [FromQuery] Language language = Language.Spanish)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid request, Id is required");
            }

            try
            {
                var recipe = await _getRecipeUseCase.ExecuteAsync(id, recipeSourceType.HasValue ? recipeSourceType.Value : RecipeSourceType.Internal, language);
                return Ok(recipe);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing the request.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<RecipeController>
        [HttpPost]
        public async Task<ActionResult<RecipeListResponse>> Post([FromBody] RecipeRequest request, CancellationToken ct)
        {
            if (!request.IsValid(out var errorMessage))
            {
                return BadRequest($"Invalid request:{errorMessage}");
            }

            var recipeResponse = new List<RecipeListResponse>();
            foreach (RecipeSourceType recipeSourceType in Enum.GetValues(typeof(RecipeSourceType)))
            {
                try
                {
                    if (Environment.GetEnvironmentVariable($"{recipeSourceType.ToString().ToLower()}_active") != "true")
                    {
                        continue;
                    }
                    var response = await _recipeSearchUseCase.ExecuteAsync(request, recipeSourceType);
                    if (response != null && response.Any())
                    {
                        recipeResponse.AddRange(response);
                    }

                }
                catch (ArgumentException ex)
                {
                    _logger.LogError(ex, "Invalid request. Ingredients are required.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while processing the request. Source Type: { recipeSourceType.ToString() }");
                }
            }

            return Ok(recipeResponse);

        }
    }
}
