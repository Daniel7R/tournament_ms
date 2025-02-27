using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TournamentMS.Domain.Enums;

namespace TournamentMS
    
    .Domain.Entities
{
    public class Tournament
    {
        public int Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int IdCategory { get; set; }
        public Category Category { get; set; }
        [Required]
        public int IdGame { get; set; }
        public Game Game { get; set; }
        [Required]
        public bool IsFree { get; set; }
        [Required]
        public int IdOrganizer { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public TournamentStatus Status { get; set; } = TournamentStatus.PENDING;
        public int? IdTeamWinnerTournament {  get; set; }
        [Required]
        public int IdPrize {  get; set; }
        public Prizes Prize { get; set; }
        public IEnumerable<TournamentUserRole> UsersTournamentRole { get; set; }
        public IEnumerable<Matches> Matches { get; set; }

    }
}
