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
    [Route("api/v1/games")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService= gameService; 
        }

        /// <summary>
        /// Get the system available games
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMatchesByTournament()
        {
            var response = new ResponseDTO<IEnumerable<GameResponseDTO>>();
            try
            {
                var categories = await _gameService.GetGamesAsync();
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
