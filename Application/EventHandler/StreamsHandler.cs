using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Domain.Enums;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.EventHandler
{
    public class StreamsHandler
    {
        private readonly ITournamentUserRoleRepository _userRoleRepo;

        private readonly IMatchesRepository _matchesrepo;

        public StreamsHandler(ITournamentUserRoleRepository userRoleService, IMatchesRepository matchesRepository)
        {
            _userRoleRepo = userRoleService;
            _matchesrepo = matchesRepository;
        }

        public async Task<ValidateMatchRoleUserResponse> ValidateRoleUserAndMatch(ValidateMatchRoleUser request)
        {
            //obtener idTorneo
            var match = await _matchesrepo.GetMatchbyId(request.IdMatch);

            if (match == null) {
                return new ValidateMatchRoleUserResponse
                {
                    IsExistingMatch = false,
                    IsValidRoleUser = false
                };
            }
            var validation = new ValidateMatchRoleUserResponse
            {
                IsExistingMatch = true,
                IsValidRoleUser = false
            };
            var userRole =await _userRoleRepo.GetUserRole(request.IdUser, match.IdTournament, Domain.Enums.EventType.TOURNAMENT);
            if (userRole.Role!=null && (userRole.Role.Equals(TournamentRoles.ADMIN)||(userRole.Role.Equals(TournamentRoles.SUBADMIN))))
            {
                validation.IsValidRoleUser = true;
            }

            return validation;
        }
    }
}
