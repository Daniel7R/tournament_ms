using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Data;

namespace TournamentMS.Infrastructure.Repository
{
    public class MatchesRepository : IMatchesRepository
    {
        private readonly TournamentDbContext _context;

        public MatchesRepository(TournamentDbContext context)
        {
            _context = context;
        }

        public async Task AddTeamsToMatch(int idMatch, List<int> idsTeams)
        {
            try
            {
                var teamsMatches = idsTeams.Select(id => new TeamsMatches
                {
                    IdTeam = id,
                    IdMatch = idMatch
                }).ToList();

                _context.TeamsMatches.AddRange(teamsMatches);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Matches> CreateMatch(Matches match)
        {
            try
            {
                _context.Matches.Add(match);
                await _context.SaveChangesAsync();

                return match;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Matches> GetMatchbyId(int id)
        {
            return await _context.Matches.Where(t => t.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Matches>> GetMatchesByIdTournament(int idTournament) => await _context.Matches.Where(match => match.IdTournament == idTournament).ToListAsync();
    
        public async Task SetWinnerMatch(int idMatch, int idTeam, MatchStatus status)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.Matches.Where(m => m.Id == idMatch)
                       .ExecuteUpdateAsync(setters => setters
                            .SetProperty(t => t.Status, status)
                            .SetProperty(m => m.IdTeamWinner, idTeam)
                );
                await transaction.CommitAsync();   
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException($"error assigning winner {ex.Message}");
            }
        }

        public async Task UpdateMatchDate(int idMatch, DateTime newDate)
        {
              using var transaction = _context.Database.BeginTransaction();
            try
            {
                await _context.Matches.Where(m => m.Id == idMatch)
                       .ExecuteUpdateAsync(setters => setters
                            .SetProperty(m => m.Date, newDate)
                );
                await transaction.CommitAsync();   
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException($"error changing date match {ex.Message}");
            }
        }
    }
}
