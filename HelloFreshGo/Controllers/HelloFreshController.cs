using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloFreshGo.Business.Managers;
using HelloFreshGo.Business.Contracts;
using Microsoft.AspNetCore.Mvc;
using HelloFreshGo.Entities.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;

namespace HelloFreshGo.Controllers
{
    [Route("api/hellofresh")]
    [ApiController]
    public class HelloFreshController : ControllerBase
    {
        private readonly IRecipeManager _recipeManager;
        public readonly ILogger<HelloFreshController> _logger;

        public HelloFreshController(IRecipeManager recipeManager, ILogger<HelloFreshController> logger)
        {
            _recipeManager = recipeManager;
            _logger = logger;
        }

        /// <summary>
        /// Retrieve all existing recipes.
        /// </summary>
        /// <returns>A string status</returns>
        // GET api/hellofresh/recipes
        [HttpGet("recipes")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(500)]
        public IActionResult ListRecipes()
        {
            try
            {
                var items = _recipeManager.GetRecipes();
                if (items.IsCompletedSuccessfully)
                {
                    var recipes = items.Result;
                    return Ok(recipes);
                }                
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new recipe and then returns a json as response for succesfully adding a recipe.
        /// </summary>
        /// <param name="model">The model of the recipe to be created</param>
        /// <returns>A string status</returns>
        [HttpPost("recipes")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Create([FromBody] Recipes model)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                var recipe = _recipeManager.Save(model); 

                if (recipe.IsCompletedSuccessfully)
                    return Ok(recipe.Result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); 
            }

            return BadRequest();
        }

        /// <summary>
        /// Retrieves a recipe by ID.
        /// </summary>
        /// <param name="id">The ID of the desired recipe</param>
        /// <returns>A json containing the required recipe</returns>
        // GET api/hellofresh/recipes/5
        [HttpGet("recipes/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetRecipe(long id)
        {
            try
            {
                var response = _recipeManager.Details(id);

                if (response != null)
                    return Ok(response);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a recipe by ID and then updates properties from the retrieved recipe.
        /// </summary>
        /// <param name="id">The ID of the desired recipe</param>
        /// <param name="update">The ID of the desired recipe</param>
        /// <returns>A json containing the updated recipe</returns>
        //PUT api/hellofresh/recipes/5               
        [HttpPut("recipes/{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdateRecipe(long id, Recipes update)
        {
            try
            {
                var recipeDetail = _recipeManager.Details(id);

                if (recipeDetail == null)
                    return NotFound();

                var item = _recipeManager.EditRecipe(id, update);
                var recipe = item.Result;

                if (item.IsCompletedSuccessfully && recipe.Id > 0)
                    return Ok(recipe);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }            
        }

        /// <summary>
        /// Retrieves a recipe by ID, rates the retrieved recipe and then saves it.
        /// </summary>
        /// <param name="id">The ID of the desired recipe</param>
        /// <param name="ratings">The model of the desired recipe</param>
        /// <returns>A json containing the rated recipe</returns>
        [HttpPost("recipes/{id}/rating")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult RateRecipe(long id, Ratings ratings)
        {
            try
            {
                if (ratings.RecipeRating > 5)
                {
                    return BadRequest("The rating given exceeds the required limit. Please rate from 1 - 5.");
                }

                var doesRecipeExist = _recipeManager.Details(id) != null 
                                       ? true : false;

                if (!doesRecipeExist)
                    return NotFound("The recipe requested was not found");

                var rateRecipe = _recipeManager.RateRecipe(id, ratings);
                var rated = rateRecipe.Result;

                if (rateRecipe.IsCompletedSuccessfully && rated.Id > 0)
                    return Ok(rated);
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a recipe by ID and then deletes the retrieved recipe.
        /// </summary>
        /// <param name="id">The ID of the desired recipe</param>
        /// <returns>A json containing the deleted recipe</returns>
        //DELETE api/hellofresh/recipes/5 
        [HttpDelete("recipes/{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult DeleteRecipe(long id)
        {
            try
            {
                var recipe = _recipeManager.Details(id);

                if (recipe == null)
                    return NotFound("The recipe requested was not found");

                var delete = _recipeManager.DeleteRecipe(id);
                var deletedRecipe = delete.Result;

                if (delete.IsCompletedSuccessfully)
                    return Ok($"{recipe.Name} has been successfully deleted");
                else
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }            
        }

        /// <summary>
        /// Filters through the list of recipes in record to retrieve the requested recipe by search.
        /// </summary>
        /// <param name="name">The name by which the user searches for a specific recipe</param>
        /// <param name="preptime">when user wamts to search for preptime of recipes in record</param>
        /// <returns>A json containing the filtered recipe</returns>
        //GET api/hellofresh/recipes/search?name=
        [HttpGet("recipes/search")]
        [AllowAnonymous]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> SearchForRecipe(string name, string preptime)
        {
            var response = await _recipeManager.SearchForRecipe(name, preptime);

            if (response != null && response.Count() > 0)
            {
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
        
    }
}