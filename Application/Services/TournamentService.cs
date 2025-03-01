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
        //private readonly IRepository<Tournament> _tournamentRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Game> _gameRepository;
        private readonly IEventBusProducer _eventBusProducer;

        private readonly IMapper _mapper;
        private const int LIMIT_FREE_TOURNAMENTS = 1;
        private int LIMIT_FREE_VIEWERS;

        //public TournamentService(IRepository<Tournament> tournamentRepository, IRepository<Game> gameRepository, IRepository<Category> categoryRepository, IMapper mapper)
        public TournamentService(ITournamentRepository tournamentRepository, IRepository<Game> gameRepository, IRepository<Category> categoryRepository, IMapper mapper,IEventBusProducer eventBus)
        {
            _tournamentRepository = tournamentRepository;
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
            _eventBusProducer = eventBus;
        }
        public async Task<TournamentResponseDTO> CreateTournamentAsync(CreateTournamentRequest tournamentDTO)
        {
            if (tournamentDTO.Name.IsNullOrEmpty()) throw new ArgumentException("Tournament name is required");

            if (tournamentDTO.StartDate <= DateTime.UtcNow.AddHours(5)) throw new ArgumentException("StartDate can't be in past");
            if (tournamentDTO.StartDate >= tournamentDTO.EndDate) throw new ArgumentException("StartDate must be before EndDate");
            
            var categoryExists = await _categoryRepository.GetByIdAsync(tournamentDTO.IdCategory);
            if (categoryExists == null) throw new ArgumentException("Category does not exist");
            
            var gameExists = await _gameRepository.GetByIdAsync(tournamentDTO.IdGame);
            if (gameExists == null) throw new ArgumentException("Game does not exits");

            if(tournamentDTO.CreatedBy == 0) throw new BusinessRuleException($"User can't be null");
            var isReachedLimitFree = await UserHasAlreadyFreeTournaments(tournamentDTO.CreatedBy);
            if (tournamentDTO.IsFree == true && isReachedLimitFree == true) throw new BusinessRuleException($"User has already created {LIMIT_FREE_TOURNAMENTS} free tournament(s)");
            
            var tournament = _mapper.Map<Tournament>(tournamentDTO);

            var response = await _tournamentRepository.AddAsync(tournament);
            var tournamentResponse = _mapper.Map<TournamentResponseDTO>(response);
            //depending on category, limit free participants
            if(tournamentResponse.IsFree == true)
            {
                tournamentResponse.MaxPlayers = categoryExists.LimitParticipant;
            }
            tournamentResponse.MaxPlayers = categoryExists.LimitParticipant;
            var generateTickets = new GenerateParticipantsTicketRequest
            {
                IdTournament = tournament.Id,
                IsFree = tournament.IsFree,
                QuantityTickets = tournamentResponse.MaxPlayers
            };
            //Genero 
            await _eventBusProducer.PublishEventAsync<GenerateParticipantsTicketRequest>(generateTickets, Queues.Queues.GENERATE_PARTICIPANTS_TICKETS_ASYNC);

            return tournamentResponse;
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

            if (request != null  && request.Count()== LIMIT_FREE_TOURNAMENTS)
            {
                hasAlreadyLimit = true;
            }
            return hasAlreadyLimit;
        }

        public async Task<IEnumerable<TournamentResponseDTO>> GetTournamentsByStatus(TournamentStatus status)
        {
            var tournaments = await _tournamentRepository.GetTournamentsByStatus(status);

            var tournamentResponse = _mapper.Map<IEnumerable<TournamentResponseDTO>>(tournaments).ToList();


            tournamentResponse.ForEach(t =>
            {
                var tournament = tournaments.FirstOrDefault(x => x.Id == t.Id);
                if (tournament != null)
                {
                    t.CategoryName = tournament.Category?.Name;
                    t.GameName = tournament.Game?.Name;
                    t.MaxPlayers = tournament.Category.LimitParticipant;
                }
            });

            return tournamentResponse;
        }
    }
}
