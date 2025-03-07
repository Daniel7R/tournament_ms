using Microsoft.EntityFrameworkCore;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class TeamsService : ITeamsService
    {
        ITeamsRepository _teamsRepo;
        IRepository<TeamsMembers> _teamsMembersRepo;

        public TeamsService(ITeamsRepository teamsRepo, IRepository<TeamsMembers> repository)
        {
            _teamsRepo = teamsRepo;
            _teamsMembersRepo = repository;
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
                MaxMembers= game.MaxPlayersPerTeam
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
            if(team.CurrentMembers == team.MaxMembers)
            {
                team.IsFull = true;
            }

            //update team
            await _teamsRepo.UpdateAsync(team);
      
            //throw new NotImplementedException();
        }
    }
}
