﻿using TournamentMS.Domain.Entities;

namespace TournamentMS.Infrastructure.Repository
{
    public interface ITournamentUserRoleRepository: IRepository<TournamentUserRole>
    {
        Task<IEnumerable<TournamentUserRole>> GetByIdTournament(int idTournament);
        Task<TournamentUserRole> GetUserRole(int userId, int idEvent);
    }
}
