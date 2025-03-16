using System.Runtime.Serialization;

namespace TournamentMS.Domain.Enums
{
    public enum TournamentStatus
    {
        [EnumMember(Value = "PENDING")]
        PENDING,
        [EnumMember(Value = "ONGOING")]
        ONGOING,
        [EnumMember(Value = "FINISHED")]
        FINISHED,
        [EnumMember(Value = "CANCELED")]
        CANCELED
    }
}
