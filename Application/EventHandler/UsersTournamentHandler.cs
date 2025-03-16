using Azure.Core;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.EventHandler
{
    public class UsersTournamentHandler
    {
        private readonly ITeamsService _teamsService;
        private readonly ILogger<UsersTournamentHandler> _logger;
        private readonly IUserTournamentRoleService _userRoleService;

        public UsersTournamentHandler(ITeamsService teamsService, ILogger<UsersTournamentHandler> logger, IUserTournamentRoleService userRoleService)
        {
            _teamsService = teamsService;
            _logger = logger;
            _userRoleService = userRoleService;
        }

        public async Task AssignTeamMemberHandler(AssignTeamMemberRequest assignation)
        {
            _logger.LogInformation($"Assign randomly user {assignation.IdUser} for tournament {assignation.IdTournament}");
            await _teamsService.AssignTeamMember(assignation);

            //add user role participant
            var userRole = new CreateUserRoleDTO
            {
                EventType = Domain.Enums.EventType.TOURNAMENT,
                IdEvent = assignation.IdTournament,
                IdUser = assignation.IdUser,
                Role = Domain.Enums.TournamentRoles.PARTICIPANT
            };
            await _userRoleService.AssignRoleUser(userRole, assignation.IdUser);
        }

        public async Task AssignRoleViewer(AssignViewerRole request)
        {
            var userRole = new CreateUserRoleDTO
            {
                EventType = Domain.Enums.EventType.MATCH,
                IdEvent = request.IdMatch,
                IdUser = request.IdUser,
                Role = Domain.Enums.TournamentRoles.VIEWER
            };
            await _userRoleService.AssignRoleUser(userRole, request.IdUser);
        }
    }
}
