
using System.Runtime.Serialization;

namespace TournamentMS.Domain.Enums
{
    public enum EventType
    {
        [EnumMember(Value ="MATCH")]
        MATCH,
        [EnumMember(Value = "TOURNAMENT")]
        TOURNAMENT
    }
}
