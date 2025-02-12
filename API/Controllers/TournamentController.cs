using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentMS.Application.DTO;
using TournamentMS.Application.Interfaces;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        protected ResponseDTO _responseDTO;

        public TournamentController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
            _responseDTO = new();
        }

        [HttpGet("tournaments")]
        public async Task<IActionResult> GetTournaments()
        {
            var tournaments = await _tournamentService.GetTournamentsAsync();

            _responseDTO.Result = tournaments;


            return Ok(_responseDTO);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTournament(int id)
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id);


            if (tournament == null)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Not tournament found";
                return NotFound(_responseDTO);
            }

            _responseDTO.Result = tournament;

            return Ok(_responseDTO);
        }

        [HttpPost("tournaments")]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest tournamentCreated)
        {
            if (!ModelState.IsValid)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Result = ModelState;
                return BadRequest(_responseDTO);
            }
            try
            {

                var tournament = await _tournamentService.CreateTournamentAsync(tournamentCreated);
                _responseDTO.Result = tournament;

                return Ok(_responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = ex.Message;
                return BadRequest(_responseDTO);
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
    }
}
