using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    /// <summary>
    ///     This class will implement the assign role to user by tournament or match
    /// </summary>
    public class UserTournamentRoleService : IUserTournamentRoleService
    {
        private readonly ILogger<UserTournamentRoleService> _logger;
        private readonly ITournamentUserRoleRepository _userRoleRepo; 

        public UserTournamentRoleService(ILogger<UserTournamentRoleService> logger, ITournamentUserRoleRepository repository)
        {
            _userRoleRepo = repository;
            _logger = logger;
        }

        /// <summary>
        /// Method to assign role to the user
        /// </summary>
        /// <param name="idUser">When user admin wants  to assig</param>
        /// <param name="idEvent"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task AssignRoleUser(CreateUserRoleDTO createUserRole, int idUser)
        {
            //validar usuario
            TournamentUserRole tournamentUserRole = new TournamentUserRole
            {
                IdUser = idUser,
                Role = createUserRole.Role
            };

            switch (createUserRole.Role)
            {
                case TournamentRoles.ADMIN:
                    tournamentUserRole.IdTournament = createUserRole.IdEvent;
                    break;
                case TournamentRoles.SUBADMIN:
                    //validate assignator role
                    await IsAdminTournament(idUser, createUserRole.IdEvent);
                    //validate max2
                    await IsAlreadyTwoSubadmin(createUserRole.IdEvent);
                    tournamentUserRole.IdTournament = createUserRole.IdEvent;
                    break;
                case TournamentRoles.VIEWER:
                    //assign role once ticket sale is confirmed
                    tournamentUserRole.IdMatch= createUserRole.IdEvent;
                    break;
                case TournamentRoles.PARTICIPANT:
                    //assign role once ticket sale is confirmed
                    tournamentUserRole.IdTournament= createUserRole.IdEvent;
                    break;
            }
            throw new NotImplementedException();
        }

        private async Task IsAdminTournament(int idUser, int idEvent) 
        {
            var response = await _userRoleRepo.GetUserRole(idUser, idEvent, EventType.TOURNAMENT);
            //only admin can assign subadmins
            if (response == null && !response.Role.Equals(TournamentRoles.ADMIN)) throw new InvalidRoleException("User has no permissions");

        }

        private async Task IsAlreadyTwoSubadmin(int idTournament)
        {
            var response = await _userRoleRepo.GetByIdTournament(idTournament);

            int count = response.Where(r => r.Role == TournamentRoles.SUBADMIN).Count();

            if(response.Count() == 2)
            {
                throw new BusinessRuleException("Tournament has already 2 subadmins");
            }
        }
    }
}
