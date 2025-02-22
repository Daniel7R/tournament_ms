using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class TeamsMatches
    {
        public int Id { get; set; }
        [Required]
        public int IdTeam { get; set; }
        public Teams Team { get; set; }
        [Required]
        public int IdMatch { get; set; }
        public Matches Match { get; set; }
    }
}
