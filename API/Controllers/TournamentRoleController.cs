using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.API.Controllers
{
    [Route("api/v1/role")]
    [ApiController]
    public class TournamentRoleController : ControllerBase
    {
        public TournamentRoleController(ITournamentRepository repository) 
        {
        }


        [Authorize]
        [HttpPost]
        [Route("")]
        public Task<IActionResult> AssignSubadmins()
        {
            //validar que el rol del usuario en el partido es el admin del torneo
         
            throw new NotImplementedException();
        }
    }
}
