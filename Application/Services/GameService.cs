using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IRepository<Game> _gameRepository;
        private readonly IMapper _mapper;
        public GameService(IRepository<Game> gameRepository, IMapper mapper) 
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
        }
        public async Task<GameResponseDTO> CreateGameAsync(CreateGameDTO gameDTO)
        {
            if (gameDTO.Name.IsNullOrEmpty())
            {
                throw new ArgumentException("Name is required");
            }

            if (gameDTO.Players <= 1)
            {
                throw new ArgumentException("A game must require at least 2 players");
            }

            var game = _mapper.Map<Game>(gameDTO);
            
            await _gameRepository.AddAsync(game);
            
            var gameResponse = _mapper.Map<GameResponseDTO>(gameDTO);
            
            return gameResponse;
        }

        public async Task<GameResponseDTO> GetGameByIdAsync(int idTournament)
        {
            var game = await _gameRepository.GetByIdAsync(idTournament);
            if (game == null) return null;

            var gameResponse = _mapper.Map<GameResponseDTO>(game);

            return gameResponse;
        }

        public async Task<IEnumerable<GameResponseDTO>> GetGamesAsync()
        {
            var games = await _gameRepository.GetAllAsync();
            var gamesResponse = games.Select(g => _mapper.Map<GameResponseDTO>(g));

            return gamesResponse;
        }
    }
}
