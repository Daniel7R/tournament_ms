using TournamentMS.Application.DTOs.Request;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.Interfaces
{
    public interface IUserTournamentRoleService
    {
        /// <summary>
        /// This will assign a specific role to a user
        /// </summary>
        /// <param name="createUserRole"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        Task AssignRoleUser(CreateUserRoleDTO createUserRole, int idUser);

        Task AssignRoleUser(int idUser, EventType eventType, int idEvent, TournamentRoles role);

    }
}
