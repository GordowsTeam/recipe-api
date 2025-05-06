using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IRecipeService recipeService, ILogger<RecipeController> logger)
        {
            _recipeService = recipeService;
            _logger = logger;
        }

        // GET: api/<RecipeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return await Task.FromResult(new string[] { "Test", "value2" });
        }

        // GET api/<RecipeController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeResponse>> Get(int id, [FromQuery] string? externalProvider = null)
        {
            var response = await _recipeService.GetRecipeByIdAsync(id, externalProvider);

            return Ok(response);
        }

        // POST api/<RecipeController>
        [HttpPost]
        public async Task<ActionResult<RecipeResponse>> Post([FromBody] RecipeRequest request, CancellationToken ct)
        {
            if (request == null || request.Ingredients == null || !request.Ingredients.Any() || request.Ingredients.Any(l => string.IsNullOrEmpty(l)))
            {
                return BadRequest("Invalid request. Ingredients are required.");
            }

            try
            {
                var response = await _recipeService.GetRecipesAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid request. Ingredients are required.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                // Log the exception (logging mechanism not shown here)
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT api/<RecipeController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RecipeController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
