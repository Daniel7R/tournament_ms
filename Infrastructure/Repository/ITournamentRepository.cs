using TournamentMS.Application.DTOs.Request;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITournamentRepository: IRepository<Tournament>
    {
        Task<IEnumerable<Tournament>> GetTournamentWithCategoriesAndGames();
        Task<Tournament> GetTournamentWithCategoriesAndGamesById(int id);
        Task<IEnumerable<Tournament>> GetFreeTournamentsByUserId(int userId);
        Task<IEnumerable<Tournament>> GetTournamentsByStatus(TournamentStatus status);
        Task<bool> AssignPrizeTournament(int idPrize, Tournament tournament);
        Task<IEnumerable<Tournament>> GetTournamentsByIds(List<int> ids);
        Task<bool> ChangeDatesTournament(int idTournament, ChangeDatesRequest dates);
        Task<IEnumerable<Tournament>> GetFullTournamentInfo(List<TournamentStatus> status);
        Task<bool> ChangeTournamentStatus(TournamentStatus status, int idTournament);
    }
}
