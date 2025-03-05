﻿using TournamentMS.Domain.Entities;
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
    }
}
