using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface ITeamsService
    {
        /// <summary>
        /// Generate the quantity teams for tournament
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task GenerateTeams(Game game, int idTournament);
        /// <summary>
        /// Assign randomly a user into an available team
        /// </summary>
        /// <param name="request"></param>
        Task AssignTeamMember(AssignTeamMemberRequest request);
        Task<IEnumerable<TeamsTournamentResponse>> GetFullInformationTeams(int idTournament);

        Task<bool> AreValidTeamsInTournament(List<int> idsTournametns, int idTournament);
        Task<Teams> GetTeamById(int idTeam);
    }
}
