using TournamentMS.Domain.Enums;

namespace TournamentMS.Domain.Entities
{
    public class TournamentUserRole
    {
        public int Id {get;set;}
        public int? IdTournament {  get; set; }
        public int? IdMatch { get; set; }
        public int IdUser{ get; set; }
        public TournamentRoles Role { get; set; }
        public Tournament Tournament { get; set; }
        public Matches Match { get; set; }
    }
}
