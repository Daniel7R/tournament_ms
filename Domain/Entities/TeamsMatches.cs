using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class TeamsMatches
    {
        public int Id { get; set; }
        public int IdTeam { get; set; }
        public Teams Team { get; set; }
        public int IdMatch { get; set; }
        public Matches Match { get; set; }
    }
}
