using TournamentMS.Application.Messages.Enums;

namespace TournamentMS.Application.Messages.Request
{
    public class GenerateParticipantsTicketRequest
    {
        public int IdTournament { get; set; }
        public bool IsFree { get; set; }
        public int QuantityTickets { get; set; }
    }
}
