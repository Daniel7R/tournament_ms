using TournamentMS.Domain.Enums;

namespace TournamentMS.Domain.Entities
{
    /// <summary>
    /// By match ended always must exist only a winner
    /// </summary>
    public class Matches
    {
        public int Id { get; set; }
        public int IdTournament { get; set; }
        public Tournament Tournament { get; set; }
        public string Name { get; set; }
        public int? IdTeamWinner {  get; set; }
        public Teams? TeamWinner {  get; set; }
        public DateTime Date { get; set; }
        public MatchStatus Status {  get; set; } = MatchStatus.PENDING;
        public IEnumerable<TeamsMatches> TeamsMatches { get; set; }
    }
}
