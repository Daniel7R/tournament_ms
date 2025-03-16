using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.DTOs.Request
{   
    public class CreateUserRoleDTO
    {
        /// <summary>
        /// MATCH OR TOURNAMENT
        /// </summary>
        public EventType EventType { get; set; }
        public int IdEvent { get; set; }
        public int IdUser {  get; set; }
        public TournamentRoles Role { get; set; }
    }
}
