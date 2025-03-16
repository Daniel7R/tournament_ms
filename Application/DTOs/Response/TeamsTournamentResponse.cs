namespace TournamentMS.Application.DTOs.Response
{
    public class TeamsTournamentResponse
    {
        public int IdTournament { get; set; }
        public int IdTeam { get; set; }
        public string TeamName { get; set; }    
        public int MaxMembers {  get; set; }
        public int CurrentMembers {  get; set; }
        public IEnumerable<UserInfo> Members { get; set; }
    }
}
