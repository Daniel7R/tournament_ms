using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITournamentRepository: IRepository<Tournament>
    {
        Task<IEnumerable<Tournament>> GetTournamentWithCategoriesAndGames();
        Task<Tournament> GetTournamentWithCategoriesAndGamesById(int id);
        Task<IEnumerable<Tournament>> GetFreeTournamentsByUserId(int userId);
    }
}
