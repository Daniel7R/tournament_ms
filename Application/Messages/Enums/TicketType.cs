using System.Runtime.Serialization;

namespace TournamentMS.Application.Messages.Enums
{
    public enum TicketType
    {
        [EnumMember(Value = "VIEWER")]
        VIEWER,
        [EnumMember(Value = "PARTICIPANT")]
        PARTICIPANT
    }
}
