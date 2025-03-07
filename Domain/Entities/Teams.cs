namespace TournamentMS.Domain.Entities
{
    public class Teams
    {
        public int Id { get; set; }
        public int IdTournament { get; set; }
        //public int IdGame { get; set; }
        public string Name { get; set; }
        public int CurrentMembers { get; set; } = 0;
        public int MaxMembers { get; set; }
        public bool IsFull { get; set; } = false;
        //prop for defined if a team has been deleted
        //public bool? IsActive { get; set; }
        public IEnumerable<TeamsMembers> Members { get; set; }
    }
}
