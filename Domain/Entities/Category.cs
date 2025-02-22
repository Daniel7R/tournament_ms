using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, MaxLength(255)]
        public string Code { get; set; }
        [Required, MaxLength(255)]
        public string Alias { get; set; }
        public int? LimitParticipant {  get; set; }
        public ICollection<Tournament> Tournaments { get; set; }
    }
}
