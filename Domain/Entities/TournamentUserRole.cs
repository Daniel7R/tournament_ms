using TournamentMS.Domain.Enums;

namespace TournamentMS.Domain.Entities
{
    public class TournamentUserRole
    {
        public int IdTournament {  get; set; }
        public Tournament Tournament { get; set; }

        public int IdUser{ get; set; }
        public TournamentRole TournamentRole { get; set; }
    }
}
