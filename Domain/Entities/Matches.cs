using TournamentMS.Domain.Enums;

namespace TournamentMS.Domain.Entities
{
    public class Matches
    {
        public int Id { get; set; }
        public int IdTournament { get; set; }
        public Tournament Tournament { get; set; }
        public int IdStream { get; set; }
        public string Name { get; set; }
        public int IdTeamWinner {  get; set; }
        public Teams TeamWinner {  get; set; }
        public DateTime Date { get; set; }
        public string Status {  get; set; } = MatchStatus.PENDING;
    }
}
