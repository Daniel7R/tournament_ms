using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.DTOs.Response
{
    public class MatchesResponseDTO
    {
        public int IdMatch { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public MatchStatus MatchStatus { get; set; }
    }
}
