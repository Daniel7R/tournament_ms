using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITeamsRepository: IRepository<Teams>
    {
        Task AddMultipleTeams(List<Teams> teams);
        Task<Teams> GetLowerMembersTeamByIdTournament(int id);
        Task<bool> UserHasAlreadyTeam(int idUser, int idTournament);
        Task<List<Teams>> GetFullInfoTeams(int idTournament);
        Task<bool> AreValidTeamsTournament(List<int> idsTeams, int idTournament);
    }
}
