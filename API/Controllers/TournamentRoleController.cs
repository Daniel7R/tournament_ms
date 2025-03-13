using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1/role")]
    [ApiController]
    public class TournamentRoleController : ControllerBase
    {
        private readonly IUserTournamentRoleService _tournamentRoleUser;
        public TournamentRoleController(IUserTournamentRoleService userTournamentRoleService) 
        {
            _tournamentRoleUser = userTournamentRoleService;
        }


        [Authorize]
        [HttpPost]
        [Route("subadmin")]
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
