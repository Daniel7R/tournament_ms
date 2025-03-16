using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Infrastructure.Repository
{
    public interface IMatchesRepository
    {
        Task<IEnumerable<Matches>> GetMatchesByIdTournament(int idTournament);
        Task<Matches> GetMatchbyId(int id);
        Task AddTeamsToMatch(int idMatch, List<int> idsTeams);
        Task<Matches> CreateMatch(Matches match);
        Task UpdateMatchDate(int idMatch, DateTime newDate);
        Task SetWinnerMatch(int idMatch, int idTeam, MatchStatus status);
    }
}
