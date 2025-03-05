using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Data;

namespace TournamentMS.Infrastructure.Repository
{
    public class TournamentUserRoleRepository : ITournamentUserRoleRepository
    {
        private readonly TournamentDbContext _context;

        public TournamentUserRoleRepository(TournamentDbContext context)
        {
            _context = context;
        }
        public async Task<TournamentUserRole> AddAsync(TournamentUserRole entity)
        {
           _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TournamentUserRole>> GetAllAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<TournamentUserRole?> GetByIdAsync(int id)
        {
            return await _context.UserRoles.FindAsync(id);
        }

        public async Task<IEnumerable<TournamentUserRole>> GetByIdTournament(int idTournament)
        {
            return await _context.UserRoles.Where(tr => tr.IdTournament == idTournament).ToListAsync();
        }

        public Task<TournamentUserRole> GetUserRole(int userId, int idEvent)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(TournamentUserRole entity)
        {
            _context.UserRoles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
