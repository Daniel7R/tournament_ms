using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
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

        public async Task AssignRoleUser(int idUser, EventType eventType, int idEvent, TournamentRoles role)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var userRole = new TournamentUserRole
                {
                    IdUser = idUser,
                    Role = role,
                };
                if (eventType.Equals(EventType.TOURNAMENT))
                {
                    userRole.IdTournament = idEvent;
                }else
                {
                    userRole.IdMatch = idEvent;
                }

                await _context.UserRoles.AddAsync(userRole);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch(InvalidOperationException ioe){
                await transaction.RollbackAsync();
                throw new  BusinessRuleException($"Error with duplicate roles: {ioe.Message}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException($"User role can't be setted: {ex.Message}");
            }
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

        public async Task<TournamentUserRole> GetUserRole(int userId, int idEvent, EventType type)
        {
            TournamentUserRole userRole = new TournamentUserRole();
            if (type.Equals(EventType.TOURNAMENT))
            {
                userRole = await _context.UserRoles.Where(ur => ur.IdUser== userId && ur.IdTournament == idEvent).FirstOrDefaultAsync();
            } else
            {
                userRole = await _context.UserRoles.Where(ur => ur.IdUser == userId && ur.IdMatch== idEvent).FirstOrDefaultAsync();
            }

            return userRole;
        }

        public async Task UpdateAsync(TournamentUserRole entity)
        {
            _context.UserRoles.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
