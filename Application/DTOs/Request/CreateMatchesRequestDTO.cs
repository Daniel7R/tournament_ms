namespace TournamentMS.Application.DTOs.Request
{
    public class CreateMatchesRequestDTO
    {
        public int IdTournament { get; set; }
        public int?IdStream { get; set; }
        public string Name { get; set; }    
        public DateTime MatchDate { get; set; }
        //teams to participate on a match
        public List<int> IdTeams {  get; set; }
    }
}
