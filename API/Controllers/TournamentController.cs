using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Application.Queues;
using TournamentMS.Domain.Exceptions;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        public TournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet]
        [Route("tournaments", Name ="GetTournaments")]
        [ProducesResponseType(200, Type =typeof(ResponseDTO<TournamentResponseDTO?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<TournamentResponseDTO?>))]
        public async Task<IActionResult> GetTournaments()
        {
            ResponseDTO<IEnumerable<TournamentResponseDTO>?> _responseDTO = new();
            try
            {
                var tournaments = await _tournamentService.GetTournamentsAsync();

                _responseDTO.Result = tournaments;

                return Ok(_responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                return BadRequest(_responseDTO);
            }
        }


        [HttpGet]
        [Route("tournaments/{id}", Name = "GetTournamentById")]
        [ProducesResponseType(200, Type =typeof(ResponseDTO<TournamentResponseDTO>))]
        [ProducesResponseType(404, Type = typeof(ResponseDTO<string?>))]
        public async Task<IActionResult> GetTournament(int id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);
            ResponseDTO<TournamentResponseDTO?> _responseDTO= new();

            if (tournament == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Not tournament found";
                return NotFound(_responseDTO);
            }

            _responseDTO.Result = tournament;

            return Ok(_responseDTO);
        }

        /*
        [HttpGet]
        [Route("testrabbit/{id}")]
        public async Task<IActionResult> TestRabbit(int id)
        {
            var request = new GetUserByIdRequest { Id = id };
            //var response = await _eventBusProducer.SendRequest<GetUserByIdRequest, GetUserByIdResponse>(request, Queues.GET_USER_BY_ID);
            return Ok(response);
        }*/
        [HttpPost]
        [Route("tournaments", Name ="CreateTournament")]
        [Authorize]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<TournamentResponseDTO?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest tournamentCreated)
        {
            ResponseDTO<ModelStateDictionary?> _responseErrorDTO = new();
            if (!ModelState.IsValid)
            {
                _responseErrorDTO.IsSuccess = false;
                _responseErrorDTO.Result = ModelState;
                return BadRequest(_responseErrorDTO);
            }
            try
            {
                ResponseDTO<TournamentResponseDTO?> _responseDTO = new();
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");

                tournamentCreated.CreatedBy = Convert.ToInt32(user);
                var tournament = await _tournamentService.CreateTournamentAsync(tournamentCreated);
                _responseDTO.Result = tournament;

                return Ok(_responseDTO);
            }
            catch(BusinessRuleException bre)
            {
                _responseErrorDTO.IsSuccess = false;
                _responseErrorDTO.Message = bre.Message;
                return BadRequest(_responseErrorDTO);
            }
            catch (Exception ex)
            {
                _responseErrorDTO.IsSuccess = false;
                _responseErrorDTO.Message = ex.Message;
                return BadRequest(_responseErrorDTO);
            }
        }

        /*

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTournament(int id, [FromBody] TournamentCreatedDTO tournamentUpdate)
        {
            var tournament = await _tournamentService.GetTournamentById(id);

            if (tournament == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Tournament does not exit";

                return NotFound(_responseDTO);
            }

            if (!ModelState.IsValid)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Result = ModelState;
                return BadRequest(_responseDTO);
            }

            await _tournamentService.UpdateTournament(tournamentUpdate, tournament.Id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournament(int id)
        {
            var tournament = await _tournamentService.GetTournamentById(id);
            if (tournament == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Not found tournament";
                return NotFound(_responseDTO);
            }
            await _tournamentService.DeleteTournament(id);

            return NoContent();


        }
        */


        private string? ExtractUserId() {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            return userId;
        }
    }
}
