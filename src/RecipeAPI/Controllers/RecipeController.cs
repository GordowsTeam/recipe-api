using System.Reflection.Emit;
using System;
using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Interfaces;
using Recipe.Core.Models;
using Recipe.Core.Enums;
using Recipe.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeSearchUseCase _recipeSearchUseCase;
        private readonly IGetRecipeUseCase _getRecipeUseCase;

        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IRecipeSearchUseCase recipeSearchUseCase, IGetRecipeUseCase getRecipeUseCase, ILogger<RecipeController> logger)
        {
            _recipeSearchUseCase = recipeSearchUseCase;
            _getRecipeUseCase = getRecipeUseCase;
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
        public async Task<ActionResult<RecipeResponse>> Get(string id, [FromQuery] string? recipeSourceType = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid request, Id is required");
            }

            if (string.IsNullOrEmpty(recipeSourceType) || !Enum.TryParse<RecipeSourceType>(recipeSourceType, out var recipeType))
            {
                return BadRequest("Invalid request, recipe source type");
            }

            try
            {
                var recipe = await _getRecipeUseCase.ExecuteAsync(id, recipeType);
                return Ok(recipe);
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "An error occurred while processing the request.");
                // Log the exception (logging mechanism not shown here)
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST api/<RecipeController>
        [HttpPost]
        public async Task<ActionResult<RecipeResponse>> Post([FromBody] RecipeRequest request, CancellationToken ct)
        {
            if (request == null || request.Ingredients == null || !request.Ingredients.Any() || request.Ingredients.Any(l => string.IsNullOrEmpty(l)))
            {
                return BadRequest("Invalid request. Ingredients are required.");
            }

            var recipeResponse = new List<RecipeResponse>();
            try
            {    
                //TODO: move this code out of the try-catch
                foreach(RecipeSourceType recipeSourceType in Enum.GetValues(typeof(RecipeSourceType)))
                {
                    if(Environment.GetEnvironmentVariable($"{recipeSourceType.ToString().ToLower()}_active") != "true")
                    {
                        continue;
                    }
                    var response = await _recipeSearchUseCase.ExecuteAsync(request, recipeSourceType);
                    
                    recipeResponse.AddRange(response);
                }
                
                return Ok(recipeResponse);
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
