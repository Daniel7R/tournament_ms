using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.DTOs.Request
{
    public class ChangeTournamentStatus
    {
        public TournamentStatus NewStatus { get; set; }
    }
}
