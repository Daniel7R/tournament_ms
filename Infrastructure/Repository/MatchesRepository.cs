using Microsoft.EntityFrameworkCore;
using TournamentMS.Domain.Entities;
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

        public async Task<IEnumerable<Matches>> GetMatchesByIdTournament(int idTournament) => await _context.Matches.Where(match => match.IdTournament == idTournament).ToListAsync();
    }
}
