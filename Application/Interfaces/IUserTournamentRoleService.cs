using TournamentMS.Application.DTOs.Request;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.Interfaces
{
    public interface IUserTournamentRoleService
    {
        /// <summary>
        /// This will assign a specific role to a user
        /// </summary>
        /// <param name="idUser">id of the user that the role would be assigned</param>
        /// <param name="idEvent">Id of the event whethever it is(match or tournament)</param>
        /// <param name="role">role to assign</param>
        /// <returns></returns>
        Task AssignRoleUser(CreateUserRoleDTO createUserRole, int idUser);

    }
}
