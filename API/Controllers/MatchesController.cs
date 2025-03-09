using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Exceptions;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1/matches")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchesService _matchesService;

        public MatchesController(IMatchesService matchesService)
        {
            _matchesService= matchesService; 
        }

        /// <summary>
        /// This method creates a tournament match
        /// Assign team to the match
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<MatchesResponseDTO?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateMatch([FromBody]CreateMatchesRequestDTO match)
        {
            ResponseDTO<MatchesResponseDTO?> responseDTO = new ResponseDTO<MatchesResponseDTO?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);

                var matchRespnse= await _matchesService.CreateMatch(match,idUser);
                responseDTO.Result = matchRespnse;
                responseDTO.Message = "Match created";
                return Ok(responseDTO);
            }
            catch (InvalidRoleException ir)
            {
                responseDTO.Message = ir.Message;

                return Unauthorized(responseDTO);
            } catch (BusinessRuleException br)
            {
                responseDTO.Message = br.Message;

                return BadRequest(responseDTO);
            }
            catch (Exception ex)
            {
                responseDTO.Message = ex.Message;

                return BadRequest(responseDTO);
            }
        }

        /// <summary>
        /// Set the match winner and change status to finish
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMatchesByTournament([FromQuery] int idTournament)
        {
            var response = new ResponseDTO<IEnumerable<MatchesResponseDTO>>();
            try
            {
                var matchesTournament = await _matchesService.GetMatchesByIdTournament(idTournament);
                response.Result = matchesTournament;
                response.Message = "Successfully requested";

                return Ok(response);

            }
            catch (Exception ex)
            {
                response.Message=ex.Message;
                return BadRequest(response);
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the match winner and change status to finish
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("winner")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        [ProducesResponseType(401)]
        public Task<IActionResult> SetWinnerMatch()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Change the match date
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        [Route("{idMatch}/date")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        [ProducesResponseType(401)]
        public Task<IActionResult> ChangeDateMatch(int idMatch)
        {
            //validar que la fecha del partido este dentro del rango de fechas del torneo
            throw new NotImplementedException();
        }

        private string? ExtractUserId()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return userId;
        }

    }
}
