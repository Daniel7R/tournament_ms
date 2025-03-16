using Microsoft.EntityFrameworkCore;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class TeamsService : ITeamsService
    {
        private readonly ITeamsRepository _teamsRepo;
        private readonly IRepository<TeamsMembers> _teamsMembersRepo;
        private readonly IEventBusProducer _eventBusProducer;

        public TeamsService(ITeamsRepository teamsRepo, IRepository<TeamsMembers> repository,IEventBusProducer eventBusProducer)
        {
            _teamsRepo = teamsRepo;
            _teamsMembersRepo = repository;
            _eventBusProducer = eventBusProducer;
        }
        /// <summary>
        /// Generate the teams empty by default  when tournament is created
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="idTournament"></param>
        /// <returns></returns>
        public async Task GenerateTeams(Game game, int idTournament)
        {
            var teams = Enumerable.Range(0, game.MaxTeams).Select(t =>
            new Teams
            {

                IdTournament = idTournament,
                IsFull = false,
                Name = $"Team {t + 1}",
                MaxMembers = game.MaxPlayersPerTeam
            }).ToList();

            await _teamsRepo.AddMultipleTeams(teams);

        }

        /// <summary>
        /// Assign randomly a user into an available team
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task AssignTeamMember(AssignTeamMemberRequest request)
        {
            //validar que el usuario no se encuentre en un equipo

            await ValidateUserNotBelonginAnotherTeam(request.IdUser, request.IdTournament);
            //query teams tournament(only get 1)
            var team = await _teamsRepo.GetLowerMembersTeamByIdTournament(request.IdTournament);
            if (team == null || team.IsFull == true) throw new BusinessRuleException("There're no available teams");

            var userAssignation = new TeamsMembers
            {
                IdTeam = team.Id,
                IdUser = request.IdUser
            };
            //Insert member to team
            var asssignation = await _teamsMembersRepo.AddAsync(userAssignation);
            //increase by 1
            team.CurrentMembers += 1;
            //check if it's full
            if (team.CurrentMembers == team.MaxMembers)
            {
                team.IsFull = true;
            }

            //update team
            await _teamsRepo.UpdateAsync(team);

            //throw new NotImplementedException();
        }


        private async Task ValidateUserNotBelonginAnotherTeam(int idUser, int idTournament)
        {
            var responseValidation = await _teamsRepo.UserHasAlreadyTeam(idUser, idTournament);

            if (responseValidation == true) throw new BusinessRuleException($"User already has a team");
        }

        public async Task<IEnumerable<TeamsTournamentResponse>> GetFullInformationTeams(int idTournament)
        {
            var teams = await _teamsRepo.GetFullInfoTeams(idTournament);

            var userIds = teams.SelectMany(t => t.Members).Select(m => m.IdUser).Distinct().ToList();

            var usersInfo = await _eventBusProducer.SendRequest<List<int>, List<GetUserByIdResponse>>(userIds, Queues.Queues.USERS_BULK_INFO);
            var usersDict = usersInfo.ToDictionary(u => u.Id);

            List<TeamsTournamentResponse> response = teams.Select(team => new TeamsTournamentResponse
            {
                IdTournament = team.IdTournament,
                IdTeam = team.Id,
                TeamName = team.Name,
                CurrentMembers = team.CurrentMembers,
                MaxMembers = team.MaxMembers,
                Members = team.Members.Select(member => 
                    usersDict.TryGetValue(member.IdUser, out var userInfo) ? 
                        new UserInfo { 
                            IdUser = member.IdUser, 
                            Username = userInfo.Name, 
                            Email = userInfo.Email 
                        }
                        : new UserInfo { 
                            IdUser = member.IdUser, 
                            Username = null 
                        }
                   ).ToList(),
            }).ToList();

            return response;
        }

        public async Task<bool> AreValidTeamsInTournament(List<int> idsTournametns, int idTournament)
        {
            var areAllValid = await _teamsRepo.AreValidTeamsTournament(idsTournametns, idTournament);
            return areAllValid;
        }

        public async Task<Teams> GetTeamById(int idTeam){
            return await _teamsRepo.GetByIdAsync(idTeam);
        }
    }
}
