using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class Game
    {
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required, Range(1, 100)]
        public int Players { get; set; }
        public ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
    }
}
