using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class TeamsMembers
    {
        public int Id { get; set; }
        [Required]
        public int IdTeam { get; set; }
        public Teams Team { get; set; }
        [Required]
        public int IdUser { get; set; }
    }
}
