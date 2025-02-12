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
        [Required, Range(2, 1000)]
        public int MaxPlayers { get; set; }
        public bool IsPaid { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Price { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = TournamentStatus.PENDING;
        //public List<int> SubAdmins { get; set; }

    }
}
