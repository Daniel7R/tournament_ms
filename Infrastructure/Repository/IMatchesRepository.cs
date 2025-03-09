using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Repository
{
    public interface IMatchesRepository
    {
        Task<IEnumerable<Matches>> GetMatchesByIdTournament(int idTournament);

        Task AddTeamsToMatch(int idMatch, List<int> idsTeams);
        Task<Matches> CreateMatch(Matches match);
    }
}
