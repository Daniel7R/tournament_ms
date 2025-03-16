using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITournamentUserRoleRepository : IRepository<TournamentUserRole>
    {
        Task<IEnumerable<TournamentUserRole>> GetByIdTournament(int idTournament);
        Task<TournamentUserRole> GetUserRole(int userId, int idEvent, EventType type);
        Task AssignRoleUser(int idUser, EventType eventType, int idEvent, TournamentRoles role);
    }
}
