using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Queues;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Service
{
    public class TournamentService : ITournamentService, ITournamentValidations
    {
        private readonly ITournamentUserRoleRepository _userTournamentRole;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IPrizeService _prizeService;
        private readonly ITeamsService _teamsService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IEventBusProducer _eventBusProducer;

        private readonly IMapper _mapper;
        private const int LIMIT_FREE_TOURNAMENTS = 1;
        private int LIMIT_FREE_VIEWERS;

        //public TournamentService(IRepository<Tournament> tournamentRepository, IRepository<Game> gameRepository, IRepository<Category> categoryRepository, IMapper mapper)
        public TournamentService(
            ITournamentRepository tournamentRepository, ITournamentUserRoleRepository repoUserRole,
            IRepository<Game> gameRepository, IRepository<Category> categoryRepository,
            IMapper mapper, IEventBusProducer eventBus,
            IPrizeService prizeService, ITeamsService teamsService)
        {
            _tournamentRepository = tournamentRepository;
            _teamsService = teamsService;
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
            _eventBusProducer = eventBus;
            _prizeService = prizeService;
            _userTournamentRole = repoUserRole;
        }
        public async Task<TournamentResponseDTO> CreateTournamentAsync(CreateTournamentRequest tournamentDTO, int idUser)
        {
            (Category categoryExists, Game gameExists) categoryAndGame = await ValidationsDateCategoryGame(tournamentDTO);

            var tournament = _mapper.Map<Tournament>(tournamentDTO);

            var createPrize = new CreatePrizeDTO { Total = tournamentDTO.Prize.Total, Description = tournamentDTO.Prize.Description };
            var prize = await CreatePrizes(createPrize);

            tournament.IdPrize = prize.Id;
            var response = await _tournamentRepository.AddAsync(tournament);
            var tournamentResponse = _mapper.Map<TournamentResponseDTO>(response);
            //depending on category, limit free participants
            tournamentResponse.MaxPlayers = categoryAndGame.gameExists.Players;
            if (tournamentResponse.IsFree == true)
            {
                tournamentResponse.MaxPlayers = categoryAndGame.categoryExists.LimitParticipant;
            }


            await GenerateUserRole(tournament, idUser);

            await GenerateTickets(tournament, tournamentResponse.MaxPlayers);

            //once created tournament generate teams
            await _teamsService.GenerateTeams(categoryAndGame.gameExists, tournament.Id);

            await SendEmailTournament(tournament, categoryAndGame.gameExists);

            return tournamentResponse;
        }

        private async Task SendEmailTournament(Tournament tournament, Game game)
        {
            var tournamentTextType = tournament.IsFree == true ? "free" : "paid";
            var payload = new EmailBulkNotificationRequest
            {
                Body = $"Brace yourself! {tournament.Name} is here since {tournament.StartDate} to {tournament.EndDate}! With ID {tournament.Id} and game {game.Name}, {tournament.Description}!🚀🚀🚀",
                Subject = "🎮New Gaming Hype🎮"
            };
            //send all system users email
            await _eventBusProducer.PublishEventAsync<EmailBulkNotificationRequest>(payload, Queues.Queues.SEND_EMAIL_CREATE_TOURNAMENT);
        }
        private async Task GenerateUserRole(Tournament tournament, int idUser)
        {
            //assigAdmin role to creator
            TournamentUserRole userRoleAdmin = new TournamentUserRole
            {
                IdTournament = tournament.Id,
                IdUser = idUser,
                Role = TournamentRoles.ADMIN
            };
            await _userTournamentRole.AddAsync(userRoleAdmin);
        }
        private async Task GenerateTickets(Tournament tournament, int quantity)
        {
            var generateTickets = new GenerateParticipantsTicketRequest
            {
                IdTournament = tournament.Id,
                IsFree = tournament.IsFree,
                QuantityTickets = quantity
            };
            //Genero 
            await _eventBusProducer.PublishEventAsync<GenerateParticipantsTicketRequest>(generateTickets, Queues.Queues.GENERATE_PARTICIPANTS_TICKETS_ASYNC);
        }
        private async Task<(Category, Game)> ValidationsDateCategoryGame(CreateTournamentRequest tournamentDTO)
        {
            //make validations and return category and service that would be used
            if (tournamentDTO.Name.IsNullOrEmpty()) throw new BusinessRuleException("Tournament name is required");

            if (tournamentDTO.StartDate <= DateTime.UtcNow.AddHours(-5)) throw new BusinessRuleException("StartDate can't be in past");
            if (tournamentDTO.StartDate >= tournamentDTO.EndDate) throw new BusinessRuleException("StartDate must be before EndDate");

            var categoryExists = await _categoryRepository.GetByIdAsync(tournamentDTO.IdCategory);
            if (categoryExists == null) throw new BusinessRuleException("Category does not exist");

            var gameExists = await _gameRepository.GetByIdAsync(tournamentDTO.IdGame);
            if (gameExists == null) throw new BusinessRuleException("Game does not exits");

            if (tournamentDTO.CreatedBy == 0) throw new BusinessRuleException($"User can't be null");
            var isReachedLimitFree = await UserHasAlreadyFreeTournaments(tournamentDTO.CreatedBy);
            if (tournamentDTO.IsFree == true && isReachedLimitFree == true) throw new BusinessRuleException($"User has already created {LIMIT_FREE_TOURNAMENTS} free tournament(s)");

            return (categoryExists, gameExists);
        }

        public async Task<TournamentResponseDTO?> GetTournamentByIdAsync(int idTournament)
        {
            var tournament = await _tournamentRepository.GetTournamentWithCategoriesAndGamesById(idTournament);

            if (tournament == null) return null;
            var tournamentResponse = _mapper.Map<TournamentResponseDTO>(tournament);

            return tournamentResponse;
        }

        public async Task<IEnumerable<TournamentResponseDTO>> GetTournamentsAsync()
        {
            var tournaments = await _tournamentRepository.GetAllAsync();

            var tournamentsResponse = tournaments.Select(t => _mapper.Map<TournamentResponseDTO>(t)).ToList();

            return tournamentsResponse;
        }

        /// <summary>
        ///  This method validates if a user has already, if it's true user has already reach limit and cancel operation, otherwise it would create
        /// </summary>
        /// <param name="idTournament">The id of </param>
        /// <returns></returns>
        public async Task<bool> UserHasAlreadyFreeTournaments(int idUser)
        {
            var request = await _tournamentRepository.GetFreeTournamentsByUserId(idUser);
            var hasAlreadyLimit = false;

            if (request != null && request.Count() == LIMIT_FREE_TOURNAMENTS)
            {
                hasAlreadyLimit = true;
            }
            return hasAlreadyLimit;
        }

        public async Task<IEnumerable<FullTournamentResponse>> GetTournamentsByStatus(List<TournamentStatus> statuses)
        {
            var tournaments = await _tournamentRepository.GetFullTournamentInfo(statuses);

            var tournamentResponse = _mapper.Map<IEnumerable<FullTournamentResponse>>(tournaments).ToList();

            if (tournamentResponse.Count() > 0)
            {
                tournamentResponse.ForEach(t =>
                {
                    var tournament = tournaments.FirstOrDefault(x => x.Id == t.Id);
                    if (tournament != null)
                    {
                        t.PrizeDescription = tournament?.Prize.Description ?? null;
                        t.TotalPrize = tournament?.Prize.Total ?? 0;
                        t.CategoryName = tournament.Category.Name;
                        t.GameName = tournament.Game.Name;
                        t.MaxPlayers = tournament.Category.LimitParticipant;
                    }
                });
            }


            return tournamentResponse;
        }
        public async Task<CreatePrizeDTO> CreatePrizeAndAssignToTournament(CreatePrizeDTO prize, int idTournament, int idUser)
        {
            //validate user is admin tournament
            var roleUser = await _userTournamentRole.GetUserRole(idUser, idTournament, EventType.TOURNAMENT);
            if (roleUser == null || !roleUser.Role.Equals(TournamentRoles.ADMIN)) throw new InvalidRoleException("User has no permissions");
            //validations tournament
            Tournament? tournament = await _tournamentRepository.GetByIdAsync(idTournament);
            if (tournament == null) throw new BusinessRuleException("Tournament does not exist");
            if (tournament.IdPrize != null) throw new BusinessRuleException("Tournament already has a prize");
            //creation
            //  
            Prizes prizeConverted = await CreatePrizes(prize);
            int idPrize = prizeConverted.Id;
            await AssignPrizeTournament(idPrize, tournament);

            return _mapper.Map<CreatePrizeDTO>(prizeConverted);
        }

        private async Task<Prizes> CreatePrizes(CreatePrizeDTO prize)
        {
            Prizes prizeConverted = _mapper.Map<Prizes>(prize);
            await _prizeService.CreatePrize(prizeConverted);

            return prizeConverted;
        }

        public async Task<bool> ChangeTournamentDate(int idUser, int idTournament, ChangeDatesRequest dates)
        {
            //validations tournament
            var roleUser = await _userTournamentRole.GetUserRole(idUser, idTournament, EventType.TOURNAMENT);
            if (roleUser==null || (!roleUser.Role.Equals(TournamentRoles.ADMIN) && !roleUser.Role.Equals(TournamentRoles.SUBADMIN))) throw new InvalidRoleException("User has no permissions");
            
            Tournament? tournament = await _tournamentRepository.GetByIdAsync(idTournament);
            if (tournament == null) throw new BusinessRuleException("Tournament does not exist");
            if (!tournament.Status.Equals(TournamentStatus.PENDING)) throw new BusinessRuleException("Tournament status is different from pending");


            if (dates.StartDate <= DateTime.UtcNow.AddHours(-5)) throw new BusinessRuleException("StartDate can't be in past");
            if (dates.StartDate >= dates.EndDate) throw new BusinessRuleException("StartDate must be before EndDate");

            var isUpdated = await _tournamentRepository.ChangeDatesTournament(idTournament, dates);

            if (isUpdated)
            {
                //new dates
                tournament.StartDate = dates.StartDate;
                tournament.EndDate = dates.EndDate;

                var emailInfo = new EmailBulkNotificationRequest
                {
                    Subject = $"📢 Important Update: Tournament Date Changed!",
                    Body = $"🚀 Exciting news! The tournament <b>{tournament.Name}</b> has been updated! 🎉<br><br>" +
                           $"📅 <b>New Dates:</b> {tournament.StartDate:MMMM dd, yyyy} - {tournament.EndDate:MMMM dd, yyyy} <br><br>" +
                           $"Make sure to update your schedule and get ready for the challenge! 🏆🔥"
                };
                await _eventBusProducer.PublishEventAsync<EmailBulkNotificationRequest>(emailInfo, Queues.Queues.SEND_EMAIL_UPDATE_TOURNAMENT);
            }

            return isUpdated;
        }

        private async Task AssignPrizeTournament(int idPrize, Tournament tournament)
        {
            await _tournamentRepository.AssignPrizeTournament(idPrize, tournament);
        }

        public async Task<bool> UpdateTournamentStatus(ChangeTournamentStatus tournamentStatus, int idTournament,int idUser)
        {
            //validar usuario rol si puede
            var roleUser = await _userTournamentRole.GetUserRole(idUser, idTournament, EventType.TOURNAMENT);
            if (roleUser == null || !roleUser.Role.Equals(TournamentRoles.ADMIN)) throw new InvalidRoleException("User has no permissions");

            Tournament? tournament = await _tournamentRepository.GetByIdAsync(idTournament);
            if (tournament == null) throw new BusinessRuleException("Tournament does not exist");

            var response = await _tournamentRepository.ChangeTournamentStatus(tournamentStatus.NewStatus, idTournament);

            if (tournamentStatus.NewStatus.Equals(TournamentStatus.FINISHED))
                //CAMBIAR ESTADO DE LOS TICKETS DE PARTICIPANTE A USED PARA INVALIDAR
                //NECESITO ID TORNEO
                await _eventBusProducer.PublishEventAsync<int>(idTournament, Queues.Queues.CHANGE_TICKETS_PARTICIPANT_USED);

            return response;
        }
    }
}
