using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Exceptions;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;
        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        /// <summary>
        /// Returns basic info about teams, such as teams names, and team members in a tournament
        /// </summary>
        /// <param name="idTournament"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("teams", Name = "GetTeamsByIdTournament")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<IEnumerable<TeamsTournamentResponse?>>))]
        [ProducesResponseType(404, Type = typeof(ResponseDTO<IEnumerable<TeamsTournamentResponse>?>))]
        public async Task<IActionResult> GetTeams([FromQuery]int idTournament)
        {
            var response = new ResponseDTO<IEnumerable<TeamsTournamentResponse>>();
            try
            {
                var teamsInfo = await _teamsService.GetFullInformationTeams(idTournament);
                response.Result = teamsInfo;
                return Ok(response);
            } catch(BusinessRuleException br)
            {
                response.Message = br.Message;
                return BadRequest(response);
            }
        }
    }
}
