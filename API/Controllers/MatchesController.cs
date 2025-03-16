using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TournamentMS.Application.DTOs;
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
        public async Task<IActionResult> GetMatchesByTournament([FromQuery, BindRequired] int idTournament)
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
        }

        /// <summary>
        /// Set the match winner and change status to finish
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        [Route("winner")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<bool>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<bool?>))]
        [ProducesResponseType(401)]
        [ProducesResponseType(500, Type = typeof(ResponseDTO<bool?>))]
        public async Task<IActionResult> SetWinnerMatch([FromBody] MatchWinnerDTO matchWinner)
        {
            var response = new ResponseDTO<bool?>();
            try{
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                var setWinner =  await _matchesService.SetWinnerMatch(matchWinner,idUser);
                response.Result= true;
                response.Message="Winner has been set";

                return Ok(response);

            } catch (BusinessRuleException br){
                response.Message= br.Message;

                return BadRequest(response);
            } catch(InvalidRoleException ir){
                response.Message =ir.Message;

                return BadRequest(response);
            } catch (Exception ex){
                response.Message =ex.Message;

                return StatusCode(500, response);
            }
        }
        /// <summary>
        /// Change the match date if provided date and math are valid, and user has enough permissions
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        [Route("date")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangeDateMatch([FromBody] ChangeMatchhDate changeMatchhDate)
        {
            //validar que la fecha del partido este dentro del rango de fechas del torneo
            var response = new ResponseDTO<bool?>();
            try{
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                var setWinner =  await _matchesService.ChangeMatchDate(changeMatchhDate,idUser);
                response.Result= true;
                response.Message="Date has been changed";

                return Ok(response);

            } catch (BusinessRuleException br){
                response.Message= br.Message;

                return BadRequest(response);
            } catch(InvalidRoleException ir){
                response.Message =ir.Message;

                return BadRequest(response);
            } catch (Exception ex){
                response.Message =ex.Message;

                return StatusCode(500, response);
            }
        }

        private string? ExtractUserId()
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return userId;
        }

    }
}
