using System.Runtime.Serialization;

namespace TournamentMS.Domain.Enums
{
    public enum MatchStatus
    {
        [EnumMember(Value = "PENDING")]
        PENDING,
        [EnumMember(Value="ONGOING")]
        ONGOING,
        [EnumMember(Value = "FINISHED")]
        FINISHED,
        [EnumMember(Value = "UNKNOWN")]
        UNKNOWN,
        [EnumMember(Value = "CANCELED")]
        CANCELED
    }
}
