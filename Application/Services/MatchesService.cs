using AutoMapper;
using TournamentMS.Application.DTOs;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Queues;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class MatchesService : IMatchesService
    {
        private readonly IMatchesRepository _matchesRepo;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITeamsService _teamsService;
        private readonly ITournamentUserRoleRepository _userTournamentRole;
        private readonly IMapper _mapper;
        private readonly IEventBusProducer _eventBusProducer;

        public MatchesService(IMatchesRepository matchesRepository, ITeamsService teamsService, ITournamentUserRoleRepository tournamentUserRoleRepository, ITournamentRepository tournamentRepository, IMapper mapper, IEventBusProducer eventBus)
        {
            _matchesRepo = matchesRepository;
            _tournamentRepository = tournamentRepository;
            _userTournamentRole = tournamentUserRoleRepository;
            _teamsService = teamsService;
            _mapper = mapper;
            _eventBusProducer = eventBus;
        }
        public async Task<MatchesResponseDTO> CreateMatch(CreateMatchesRequestDTO match2Create, int idUser)
        {
            //validates user is admin or subadmin
            var roleUser = await _userTournamentRole.GetUserRole(idUser, match2Create.IdTournament, EventType.TOURNAMENT);
            var tournament = await _tournamentRepository.GetByIdAsync(match2Create.IdTournament);
            if (tournament == null) throw new BusinessRuleException("Tournament does not exist");
            if (roleUser == null || (!roleUser.Role.Equals(TournamentRoles.ADMIN) && !roleUser.Role.Equals(TournamentRoles.SUBADMIN))) throw new InvalidRoleException("User has no permissions");


            //validates limit when free
            if (tournament != null && tournament.IsFree == true) await ValidateMax2FreeMatchesTournament(tournament.Id);

            //validate match is in range
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var isValidDate = IsDateInRange(match2Create.MatchDate, tournament.StartDate, tournament.EndDate);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (isValidDate == false) throw new BusinessRuleException($"Match date is not valid, tournament startdate: {tournament.StartDate} and endDate: {tournament.EndDate}, but provided  was {match2Create.MatchDate}");

            //VALIDATE TEAMS TO ASSIGN TO MATCH BELONGS TO TOURNAMENT
            await ValidateTeamsExistInTournament(match2Create.IdTeams, match2Create.IdTournament);

            var match = _mapper.Map<Matches>(match2Create);
            match.Date = match2Create.MatchDate;

            var matchCreated = await _matchesRepo.CreateMatch(match);

            //ASSIGN TEAMS_MATCHES
            await _matchesRepo.AddTeamsToMatch(matchCreated.Id, match2Create.IdTeams);

            var matchResponse = _mapper.Map<MatchesResponseDTO>(matchCreated);

            return matchResponse;
        }

        public async Task<MatchesResponseDTO> GetMatchById(int idMatch)
        {
            var match = await _matchesRepo.GetMatchbyId(idMatch);
            var resposne = _mapper.Map<MatchesResponseDTO>(match);

            return resposne;
        }

        public async Task<IEnumerable<MatchesResponseDTO>> GetMatchesByIdTournament(int idTournament)
        {
            var matchesTournament = await _matchesRepo.GetMatchesByIdTournament(idTournament);

            var matchesResponse = _mapper.Map<IEnumerable<MatchesResponseDTO>>(matchesTournament);

            return matchesResponse;
        }

        private bool IsDateInRange(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck <= endDate;
        }

        /// <summary>
        /// When tournament is Free, this validates, that user cannot create more than 2 events/matches
        /// </summary>
        /// <param name="idTournament"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task ValidateMax2FreeMatchesTournament(int idTournament)
        {
            var matches = await _matchesRepo.GetMatchesByIdTournament(idTournament);

            if (matches.Count() >= 2) throw new BusinessRuleException("Free tournament has already 2 created events");
        }

        private async Task ValidateTeamsExistInTournament(List<int> idsTeams, int idTournament)
        {
            var areAllValid = await _teamsService.AreValidTeamsInTournament(idsTeams, idTournament);
            if (areAllValid == false) throw new BusinessRuleException("Check teams, there may be one or more that does not belongs to the tournament");
        }

        public async Task<bool> SetWinnerMatch(MatchWinnerDTO matchWinner, int idUser)
        {
            if (matchWinner == null || matchWinner.IdWinner == null || matchWinner.IdMatch == null) throw new BusinessRuleException("Check params, something is wrong");
            var match = await _matchesRepo.GetMatchbyId((int)matchWinner.IdMatch);
            var winner = await _teamsService.GetTeamById((int)matchWinner.IdWinner);


            if (match == null|| match.Id ==0) throw new BusinessRuleException("Match does not exist");
            if (winner == null || winner.Id == 0) throw new BusinessRuleException("Team does not exist");


            var roleUser = await _userTournamentRole.GetUserRole(idUser, match.IdTournament, EventType.TOURNAMENT);
            if (roleUser == null || (!roleUser.Role.Equals(TournamentRoles.ADMIN) && !roleUser.Role.Equals(TournamentRoles.SUBADMIN))) throw new InvalidRoleException("User has no permissions");

            if (match.Status.Equals(MatchStatus.FINISHED) || match.Status.Equals(MatchStatus.UNKNOWN) || match.Status.Equals(MatchStatus.CANCELED)) throw new BusinessRuleException("Tournament status does allow to perform this action");

            if (match.IdTeamWinner != null) throw new BusinessRuleException("Match already has a winnner");

            await _matchesRepo.SetWinnerMatch(match.Id,(int)matchWinner.IdWinner, MatchStatus.FINISHED);
            var Email = new EmailBulkNotificationRequest
            {
                Subject = "🥇Match Winner🥇",
                Body = $"We're happy to announce our match {match.Name} winner🥇 {winner.Name}!!!🥳🙌"
            };
            //CANCELAR TODOS LOS TICKETS DE VIEWERS asociados al partido
            await _eventBusProducer.PublishEventAsync<int>(match.Id, Queues.Queues.CHANGE_TICKETS_VIEWERS_USED);
            await _eventBusProducer.PublishEventAsync<EmailBulkNotificationRequest>(Email, Queues.Queues.SEND_EMAIL_MATCH_WINNER);
            return true;
        }

        public async Task<bool> ChangeMatchDate(ChangeMatchhDate changeMatchhDate, int idUser){
            
            if (changeMatchhDate== null) throw new BusinessRuleException("Check params, something is wrong");
            var match = await _matchesRepo.GetMatchbyId(changeMatchhDate.IdMatch);
            
            if (match == null || match.Id ==0) throw new BusinessRuleException("Match does not exist");

            var tournament = await _tournamentRepository.GetTournamentWithCategoriesAndGamesById(match.IdTournament);
            if(tournament== null || tournament.Id== 0) throw new BusinessRuleException("Tournament does not exist");
            
            var roleUser = await _userTournamentRole.GetUserRole(idUser, match.IdTournament, EventType.TOURNAMENT);
            if (roleUser == null || (!roleUser.Role.Equals(TournamentRoles.ADMIN) && !roleUser.Role.Equals(TournamentRoles.SUBADMIN))) throw new InvalidRoleException("User has no permissions");


            var isValidDate = IsDateInRange(changeMatchhDate.Date, tournament.StartDate, tournament.EndDate);
            if (isValidDate == false) throw new BusinessRuleException($"Match date is not valid, tournament startdate: {tournament.StartDate} and endDate: {tournament.EndDate}, but provided  was {changeMatchhDate.Date}");

            await _matchesRepo.UpdateMatchDate(match.Id, changeMatchhDate.Date);
             var Email = new EmailBulkNotificationRequest
            {
                Subject = "⬆️Match Update⬆️",
                Body = $"We have an important announce of our match {match.Name}, date has been changed to {changeMatchhDate.Date}!!!🥳🙌"
            };
            await _eventBusProducer.PublishEventAsync<EmailBulkNotificationRequest>(Email, Queues.Queues.SEND_EMAIL_MATCH_WINNER);
          
            return true;
        }
    }
}
