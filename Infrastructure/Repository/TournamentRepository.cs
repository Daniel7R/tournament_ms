using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Infrastructure.Data;

namespace TournamentMS.Infrastructure.Repository
{
    public class TournamentRepository :ITournamentRepository
    {
        private readonly TournamentDbContext _context;
        public TournamentRepository(TournamentDbContext context) 
        { 
            _context = context;
        }
        public async Task<Tournament> AddAsync(Tournament entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<Tournament>> GetAllAsync()
        {
            return await _context.Tournaments.ToListAsync();
        }

        public async Task<Tournament?> GetByIdAsync(int id)
        {
            return await _context.Tournaments.FindAsync(id);
        }

        public Task UpdateAsync(Tournament entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tournament>> GetTournamentWithCategoriesAndGames()
        {
            return await _context.Tournaments
                .Include(t => t.Game)
                .Include(t => t.Category)
                .ToListAsync();
        }

        public async Task<Tournament> GetTournamentWithCategoriesAndGamesById(int id)
        {
            return await _context.Tournaments.Include(c => c.Category).Include(g => g.Game).FirstAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tournament>> GetFreeTournamentsByUserId(int userId)
        {
            return await _context.Tournaments.Where(t => t.IsFree == true && t.IdOrganizer == userId).ToListAsync();
        }

        public async Task<IEnumerable<Tournament>> GetTournamentsByStatus(TournamentStatus status)
        {
            return await _context.Tournaments
                .Include(t => t.Game)
                .Include(t => t.Category)
                .Where(t => t.Status== status)
                .ToListAsync();
        }
    }
}
