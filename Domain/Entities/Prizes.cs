using System.ComponentModel.DataAnnotations;

namespace TournamentMS.Domain.Entities
{
    public class Prizes
    {
        public int Id { get; set; }
        //[Required]
        //public int IdTournament { get; set; }
        public Tournament Tournament { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public double Total {  get; set; }
    }
}
