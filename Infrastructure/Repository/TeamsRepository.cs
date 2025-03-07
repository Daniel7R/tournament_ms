using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Data;

namespace TournamentMS.Infrastructure.Repository
{
    public class TeamsRepository : ITeamsRepository
    {

        private readonly TournamentDbContext _context;
        private readonly DbSet<Teams> _dbSet;

        public TeamsRepository(TournamentDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Teams>();
        }

        public async Task<Teams> AddAsync(Teams entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task AddMultipleTeams(List<Teams> teams)
        {
            _context.AddRange(teams);

            await _context.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Teams>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<Teams?> GetByIdAsync(int id)
        {
            return await _context.Teams.FirstOrDefaultAsync(t => t.Id == id);  
        }

        public async Task<Teams?> GetLowerMembersTeamByIdTournament(int id)
        {
            return await _context.Teams.Where(t => t.IdTournament == id && !t.IsFull)
                .OrderBy(t => t.CurrentMembers)//priorize teams with less members
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Teams entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
