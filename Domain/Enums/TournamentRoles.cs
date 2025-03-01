using System.Runtime.Serialization;

namespace TournamentMS.Domain.Enums
{
    public enum TournamentRoles
    {
        [EnumMember(Value = "ADMIN")]
        ADMIN,
        [EnumMember(Value = "SUBADMIN")]
        SUBADMIN,
        [EnumMember(Value = "PARTICIPANT")]
        PARTICIPANT,
        [EnumMember(Value = "VIEWER")]
        VIEWER
    }
}
