using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;

namespace TournamentMS.Application.EventHandler
{
    public class TeamMembershandler
    {
        private readonly ITeamsService _teamsService;
        private readonly ILogger<TeamMembershandler> _logger;

        public TeamMembershandler(ITeamsService teamsService, ILogger<TeamMembershandler> logger)
        {
            _teamsService = teamsService;
            _logger = logger;
        }

        public async Task AssignTeamMemberHandler(AssignTeamMemberRequest assignation)
        {
            _logger.LogInformation($"Assign randomly user {assignation.IdUser} for tournament {assignation.IdTournament}");
            await _teamsService.AssignTeamMember(assignation);
        }
    }
}
