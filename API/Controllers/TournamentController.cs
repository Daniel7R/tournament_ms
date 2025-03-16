using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IUserTournamentRoleService _tournamentRoleUser;

        public TournamentController(ITournamentService tournamentService, IUserTournamentRoleService userTournamentRoleService)
        {
            _tournamentService = tournamentService;
            _tournamentRoleUser = userTournamentRoleService;
        }

        /// <summary>
        /// Creates a new tournament in system, if free, it validates, that a user can't create more than one free tournament
        /// </summary>
        /// <param name="tournamentCreated"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("tournaments", Name = "CreateTournament")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<TournamentResponseDTO?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<object?>))]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateTournament([FromBody] CreateTournamentRequest tournamentCreated)
        {
            ResponseDTO<ModelStateDictionary?> _responseErrorDTO = new();
            if (!ModelState.IsValid)
            {
                _responseErrorDTO.Result = ModelState;
                return BadRequest(_responseErrorDTO);
            }
            try
            {
                ResponseDTO<TournamentResponseDTO?> _responseDTO = new();
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");

                tournamentCreated.CreatedBy = Convert.ToInt32(user);
                var tournament = await _tournamentService.CreateTournamentAsync(tournamentCreated, Convert.ToInt32(user));
                _responseDTO.Result = tournament;
                _responseDTO.Message = "Tournament successfully created";

                return Ok(_responseDTO);
            }
            catch (BusinessRuleException bre)
            {
                _responseErrorDTO.Message = bre.Message;
                return BadRequest(_responseErrorDTO);
            }
            catch (Exception ex)
            {
                _responseErrorDTO.Message = ex.Message;
                return BadRequest(_responseErrorDTO);
            }
        }

        /// <summary>
        /// Get the available tournaments according the provided statuses
        /// </summary>
        /// <param name="statuses"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("tournaments", Name = "GetTournaments")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<FullTournamentResponse?>))]
        [ProducesResponseType(400, Type = typeof(ResponseDTO<FullTournamentResponse?>))]
        public async Task<IActionResult> GetTournaments([FromQuery] List<TournamentStatus> statuses)
        {
            ResponseDTO<IEnumerable<FullTournamentResponse>?> _responseDTO = new();
            if (statuses is null || statuses.Count == 0)
            {
                return BadRequest(new ResponseDTO<FullTournamentResponse?>
                {
                    Message = "At least one status must be provided."
                });
            }
            try
            {
                var tournaments = await _tournamentService.GetTournamentsByStatus(statuses);

                _responseDTO.Result = tournaments;
                _responseDTO.Message = "Successfully requested";

                return Ok(_responseDTO);
            }
            catch (Exception ex)
            {
                _responseDTO.Message = ex.Message;
                return BadRequest(_responseDTO);
            }
        }

        /// <summary>
        /// Change the tournament dates, remember, date must be higher than request date
        /// </summary>
        /// <param name="idTournament"></param>
        /// <param name="changeDates"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        [Route("tournaments/{idTournament}/date", Name = "ChangeDate")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(404, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(401, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(statusCode: StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangeDates(int idTournament, [FromBody]ChangeDatesRequest changeDates)
        {
            ResponseDTO<string?> response = new ResponseDTO<string?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                var updateDates=await  _tournamentService.ChangeTournamentDate(idUser, idTournament,changeDates);
                response.Message = "Tournament dates successfully changed";
                if(!updateDates)
                {
                    response.Message = "Tournament date could not be changed";
                }
                return Ok(response);
             }
            catch(InvalidRoleException ir)
            {
                response.Message = ir.Message;

                return BadRequest(response);
            }
            catch(BusinessRuleException br)
            {
                response.Message = br.Message;
                return BadRequest(response);
            }
        }

        /* cambiarlo por un metodo que modifique el premio
        [HttpPost]
        [Route("tournaments/{idTournament}/prize")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<CreatePrizeDTO?>))]
        [ProducesResponseType(404, Type = typeof(ResponseDTO<string?>))]
        [ProducesResponseType(401, Type = typeof(ResponseDTO<string?>))]
        public async Task<IActionResult> AssignTournamentPrize(int idTournament, [FromBody] CreatePrizeDTO prize)
        {
            var response = new ResponseDTO<CreatePrizeDTO>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);

                CreatePrizeDTO prizeCreated = await _tournamentService.CreatePrizeAndAssignToTournament(prize, idTournament, idUser);
                response.Message = "Successfully assigned";
                return Ok(response);
            }
            catch (BusinessRuleException ex) 
            {
                response.Message = ex.Message;

                return BadRequest(response);
            } catch(InvalidRoleException re)
            {
                response.Message = re.Message;
                return Unauthorized(response);
            }
        }
        */

        /// <summary>
        /// Change the tournament status to a new valid Status from the system
        /// </summary>
        /// <param name="idTournament"></param>
        /// <param name="tournamentStatus"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPatch]
        [Route("tournaments/{idTournament}/status", Name = "ChangeTournamentStatus")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(ResponseDTO<bool?>))]
        [ProducesResponseType(404, Type = typeof(ResponseDTO<bool?>))]
        [ProducesResponseType(401, Type = typeof(ResponseDTO<bool?>))]
        public async Task<IActionResult> ChangeTournamentStatus(int idTournament,[FromBody] ChangeTournamentStatus tournamentStatus)
        {
            var response = new ResponseDTO<bool?>();
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                var statusIsChanged = await _tournamentService.UpdateTournamentStatus(tournamentStatus, idTournament, idUser);
                response.Message = "Status changed successfully";
                if (statusIsChanged == false)
                {
                    response.Message = "Status could not be changed";
                }
                response.Result = statusIsChanged;

                return Ok(response);

            } catch(InvalidRoleException ir)
            {
                response.Message = ir.Message;
                return Unauthorized(response);

            } catch(BusinessRuleException be)
            {
                response.Message = be.Message;
                return BadRequest(response);
            }
        }
        

        /// <summary>
        /// Add a new subadmin to tournament, to help admin manage tournament and events(can't be more then 2 subadmins by tournament)
        /// </summary>
        /// <param name="createSub"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("role/subadmin")]
        public async Task<IActionResult> AssignSubadmins(CreateSubadminRequest createSub)
        {
            var response = new ResponseDTO<string?>();
            //validar que el rol del usuario en el partido es el admin del torneo
            try
            {
                var user = ExtractUserId();
                if (string.IsNullOrEmpty(user)) throw new BusinessRuleException("Invalid User");
                int idUser = Convert.ToInt32(user);
                await _tournamentRoleUser.AddSubAdmin(createSub, idUser);
                response.Message = "Successfully added";

                return Ok(response);
            } catch(BusinessRuleException br)
            {
                response.Message = br.Message;
                return BadRequest(response);
            } catch(InvalidRoleException ir)
            {
                response.Message = ir.Message;
                return BadRequest(response);
            }
             catch(Exception ex)
            {
                response.Message = ex.Message;
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
