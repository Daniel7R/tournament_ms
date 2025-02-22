namespace TournamentMS.Domain.Entities
{
    public class Teams
    {
        public int Id { get; set; }
        public int IdTournament { get; set; }
        public int IdGame { get; set; }
        public string Name { get; set; }
        public bool IsFull { get; set; } = false;
    }
}
