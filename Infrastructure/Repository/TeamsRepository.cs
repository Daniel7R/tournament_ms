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

        public async Task<bool> UserHasAlreadyTeam(int idUser, int idTournament)
        {
            var response = await _context.Teams.Where(t => t.IdTournament == idTournament)
                .Include(t => t.Members)
                .Where(tm => tm.Members.Any(m => m.IdUser == idUser)).CountAsync();
            if (response > 0)
            {
                return true;
            }

            return false;
        }

        public async Task UpdateAsync(Teams entity)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }

        /// <summary>
        /// This method returns the teams with its members by tournament id
        /// </summary>
        /// <param name="idTournament"></param>
        /// <returns></returns>
        public async Task<List<Teams>> GetFullInfoTeams(int idTournament)
        {
            var response = await _context.Teams.Where(t => t.IdTournament == idTournament)
                .Include(t => t.Members).ToListAsync();
            
            return response;
        }

        public async Task<bool> AreValidTeamsTournament(List<int> idsTeams,int idTournament)
        {
            var existingTeams = await _context.Teams.Where(team => 
                team.IdTournament == idTournament && 
                idsTeams.Contains(team.Id))
                .Select(team => team.Id).ToListAsync();

            return idsTeams.All(id => existingTeams.Contains(id));
        }
    }
}
