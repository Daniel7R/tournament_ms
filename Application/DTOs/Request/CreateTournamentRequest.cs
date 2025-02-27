using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TournamentMS.Application.DTOs.Request
{
    public class CreateTournamentRequest
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int IdCategory { get; set; }
        [Required]
        public int IdGame { get; set; }

        public int MaxPlayers { get; set; }
        [Required]
        public bool IsFree { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
