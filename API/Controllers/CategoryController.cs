using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TournamentMS.Application.DTOs;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService= categoryService; 
        }

        /// <summary>
        /// Get the system available categories
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMatchesByTournament()
        {
            var response = new ResponseDTO<IEnumerable<CategoryResponseDTO>>();
            try
            {
                var categories = await _categoryService.GetCategoriesAsync();
                response.Result = categories;
                response.Message = "Successfully requested";

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Message=ex.Message;
                return BadRequest(response);
            }
        }


    }
}
