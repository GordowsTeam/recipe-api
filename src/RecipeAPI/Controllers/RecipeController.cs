using Microsoft.AspNetCore.Mvc;
using Recipe.Application.Dtos;
using Recipe.Application.Interfaces;
using Recipe.Application.Services;
using Recipe.Application.Validators;
using Recipe.Core.Enums;
using Recipe.Domain.Enums;

namespace RecipeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController(
        IRecipeSearchUseCase recipeSearchUseCase,
        IGetRecipeUseCase getRecipeUseCase,
        IGetMyRecipesUseCase getMyRecipesUseCase,
        IAddMyRecipeUseCase addMyRecipeUseCase,
        IRemoveMyRecipeUseCase removeMyRecipeUseCase,
        ICreateUserRecipeUseCase createUserRecipeUseCase,
        IGetFavoriteRecipesUseCase getFavoriteRecipesUseCase,
        IAddFavoriteRecipeUseCase addFavoriteRecipeUseCase,
        IRemoveFavoriteRecipeUseCase removeFavoriteRecipeUseCase,
        ILogger<RecipeController> logger) : ControllerBase
    {
        private readonly IRecipeSearchUseCase _recipeSearchUseCase = recipeSearchUseCase;
        private readonly IGetRecipeUseCase _getRecipeUseCase = getRecipeUseCase;
        private readonly IGetMyRecipesUseCase _getMyRecipesUseCase = getMyRecipesUseCase;
        private readonly IAddMyRecipeUseCase _addMyRecipeUseCase = addMyRecipeUseCase;
        private readonly IRemoveMyRecipeUseCase _removeMyRecipeUseCase = removeMyRecipeUseCase;
        private readonly ICreateUserRecipeUseCase _createUserRecipeUseCase = createUserRecipeUseCase;
        private readonly IGetFavoriteRecipesUseCase _getFavoriteRecipesUseCase = getFavoriteRecipesUseCase;
        private readonly IAddFavoriteRecipeUseCase _addFavoriteRecipeUseCase = addFavoriteRecipeUseCase;
        private readonly IRemoveFavoriteRecipeUseCase _removeFavoriteRecipeUseCase = removeFavoriteRecipeUseCase;
        private readonly ILogger<RecipeController> _logger = logger;

        // GET: api/recipe/favorites
        [HttpGet("favorites")]
        public async Task<ActionResult<IReadOnlyList<RecipeDetailResponse>>> GetFavorites(CancellationToken ct)
        {
            try
            {
                var recipes = await _getFavoriteRecipesUseCase.ExecuteAsync(ct);
                return Ok(recipes);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading favorites.");
                return StatusCode(500, "An error occurred while loading your favorites.");
            }
        }

        // POST: api/recipe/favorites
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

        // DELETE: api/recipe/favorites/{recipeId}
        [HttpDelete("favorites/{recipeId}")]
        public async Task<ActionResult> RemoveFavorite(string recipeId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(recipeId))
                return BadRequest("RecipeId is required.");

            try
            {
                await _removeFavoriteRecipeUseCase.ExecuteAsync(recipeId, ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing favorite.");
                return StatusCode(500, "An error occurred while removing the favorite.");
            }
        }

        // GET: api/recipe/my-recipes
        [HttpGet("my-recipes")]
        public async Task<ActionResult<IReadOnlyList<RecipeDetailResponse>>> GetMyRecipes(CancellationToken ct)
        {
            try
            {
                var recipes = await _getMyRecipesUseCase.ExecuteAsync(ct);
                return Ok(recipes);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading my recipes.");
                return StatusCode(500, "An error occurred while loading your recipes.");
            }
        }

        // POST: api/recipe/my-recipes
        [HttpPost("my-recipes")]
        public async Task<ActionResult> AddMyRecipe([FromBody] AddMyRecipeRequest request, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(request?.RecipeId))
                return BadRequest("RecipeId is required.");

            try
            {
                await _addMyRecipeUseCase.ExecuteAsync(request.RecipeId, request.RecipeSourceType, ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding recipe to my recipes.");
                return StatusCode(500, "An error occurred while adding the recipe.");
            }
        }

        // POST: api/recipe/my-recipes/create
        [HttpPost("my-recipes/create")]
        public async Task<ActionResult<RecipeDetailResponse>> CreateUserRecipe([FromBody] CreateUserRecipeRequest request, CancellationToken ct)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
                return BadRequest("Recipe name is required.");

            try
            {
                var recipe = await _createUserRecipeUseCase.ExecuteAsync(request, ct);
                return CreatedAtAction(nameof(Get), new { id = recipe.Id }, recipe);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recipe.");
                return StatusCode(500, "An error occurred while creating the recipe.");
            }
        }

        // DELETE: api/recipe/my-recipes/{recipeId}
        [HttpDelete("my-recipes/{recipeId}")]
        public async Task<ActionResult> RemoveMyRecipe(string recipeId, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(recipeId))
                return BadRequest("RecipeId is required.");

            try
            {
                await _removeMyRecipeUseCase.ExecuteAsync(recipeId, ct);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("User ID is required. Send X-User-Id header or use authenticated session.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing recipe from my recipes.");
                return StatusCode(500, "An error occurred while removing the recipe.");
            }
        }

        // GET: api/<RecipeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            return await Task.FromResult(new string[] { "Test", "value2" });
        }

        // GET api/recipe/latest?count=5
        [HttpGet("latest")]
        public async Task<ActionResult<IReadOnlyList<RecipeListResponse>>> GetLatest([FromQuery] int count = 5, [FromQuery] Language language = Language.Spanish, CancellationToken ct = default)
        {
            if (count < 1 || count > 100)
                return BadRequest("Count must be between 1 and 100.");

            try
            {
                var request = new RecipeRequest { GetLatestCount = count, Language = language };
                var recipes = await _recipeSearchUseCase.ExecuteAsync(request, RecipeSourceType.Internal);
                if (recipes == null)
                    return Ok(Array.Empty<RecipeListResponse>());
                return Ok(recipes.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading latest recipes.");
                return StatusCode(500, "An error occurred while loading latest recipes.");
            }
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
                var recipe = await _getRecipeUseCase.ExecuteAsync(id, RecipeSourceType.Internal, language);
                if (recipe == null)
                    return NotFound();
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
            var activeSources = GetActiveRecipeSources();

            foreach (var recipeSourceType in activeSources)
            {
                try
                {
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
                    _logger.LogError(ex, "Error processing request. Source: {Source}", recipeSourceType);
                }
            }

            return Ok(recipeResponse);
        }

        /// <summary>
        /// Returns which recipe sources to query. If no env var like "internal_active=true" is set, defaults to Internal so search works.
        /// </summary>
        private static IEnumerable<RecipeSourceType> GetActiveRecipeSources()
        {
            var all = (RecipeSourceType[])Enum.GetValues(typeof(RecipeSourceType));
            var active = all
                .Where(st => st != RecipeSourceType.None &&
                    string.Equals(Environment.GetEnvironmentVariable($"{st.ToString().ToLowerInvariant()}_active"), "true", StringComparison.OrdinalIgnoreCase))
                .ToList();
            if (active.Count == 0)
                return new[] { RecipeSourceType.Internal };
            return active;
        }
    }
}
