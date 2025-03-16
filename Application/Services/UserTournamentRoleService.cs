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

        public async Task AddSubAdmin(CreateSubadminRequest request, int idAdmin)
        {
            CreateUserRoleDTO create = new CreateUserRoleDTO { EventType = EventType.TOURNAMENT, IdEvent = request.IdTournament, IdUser = request.IdUser, Role = TournamentRoles.SUBADMIN };

            await AssignRoleUser(create, idAdmin);
        }

        /// <summary>
        /// Method to assign role to the user
        /// </summary>
        /// <param name="createUserRole"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
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
                    await AssignRoleUser(createUserRole.IdUser, EventType.TOURNAMENT, createUserRole.IdEvent, TournamentRoles.SUBADMIN);
                    break;
                case TournamentRoles.VIEWER:
                    //assign role once ticket sale is confirmed
                    tournamentUserRole.IdMatch= createUserRole.IdEvent;
                    await AssignRoleUser(createUserRole.IdUser, EventType.MATCH, createUserRole.IdEvent, TournamentRoles.VIEWER);
                    break;
                case TournamentRoles.PARTICIPANT:
                    //assign role once ticket sale is confirmed
                    tournamentUserRole.IdTournament= createUserRole.IdEvent;
                    break;
            }
            if(!createUserRole.Role.Equals(TournamentRoles.SUBADMIN)&&!createUserRole.Role.Equals(TournamentRoles.VIEWER))
                await _userRoleRepo.AddAsync(tournamentUserRole);
            //throw new NotImplementedException();
        }

        public async Task AssignRoleUser(int idUser, EventType eventType, int idEvent, TournamentRoles role)
        {
            await _userRoleRepo.AssignRoleUser(idUser, eventType, idEvent, role);
            //throw new NotImplementedException();
        }

        private async Task IsAdminTournament(int idUser, int idEvent) 
        {
            var response = await _userRoleRepo.GetUserRole(idUser, idEvent, EventType.TOURNAMENT);
            //only admin can assign subadmins
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (response == null && !response.Role.Equals(TournamentRoles.ADMIN)) throw new InvalidRoleException("User has no permissions");
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        }

        private async Task IsAlreadyTwoSubadmin(int idTournament)
        {
            var response = await _userRoleRepo.GetByIdTournament(idTournament);

            int count = response.Where(r => r.Role == TournamentRoles.SUBADMIN && r.IdTournament== idTournament).Count();

            if(response.Count() >= 2)
            {
                throw new BusinessRuleException("Tournament has already 2 subadmins");
            }
        }
    }
}
