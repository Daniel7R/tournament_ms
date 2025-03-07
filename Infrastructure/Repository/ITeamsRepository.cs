using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITeamsRepository: IRepository<Teams>
    {
        Task AddMultipleTeams(List<Teams> teams);
        Task<Teams> GetLowerMembersTeamByIdTournament(int id);
    }
}
