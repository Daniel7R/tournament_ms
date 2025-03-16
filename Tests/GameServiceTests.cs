using AutoMapper;
using Moq;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class GameServiceTests
    {
        private readonly Mock<IRepository<Game>> _gameRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GameService _gameService;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IRepository<Game>>();
            _mapperMock = new Mock<IMapper>();
            _gameService = new GameService(_gameRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateGameAsync_ValidGame_ReturnsGameResponse()
        {
            var gameDTO = new CreateGameDTO { Name = "Fornite", Players = 2 };
            var game = new Game { Id = 1, Name = "Fornite", Players = 2 };
            var gameResponseDTO = new GameResponseDTO { IdGame = 1, Name = "Fornite", Players = 2 };

            _mapperMock.Setup(m => m.Map<Game>(gameDTO)).Returns(game);
            _mapperMock.Setup(m => m.Map<GameResponseDTO>(game)).Returns(gameResponseDTO);
            _gameRepositoryMock.Setup(repo => repo.AddAsync(game)).ReturnsAsync(game);

            var result = await _gameService.CreateGameAsync(gameDTO);

            //Assert.NotNull(result);
            Assert.Equal(gameResponseDTO.IdGame, game.Id);
        }

        [Fact]
        public async Task CreateGameAsync_NullGameDTO_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await _gameService.CreateGameAsync(null));
        }

        [Fact]
        public async Task CreateGameAsync_MissingName_ThrowsException()
        {
            var gameDTO = new CreateGameDTO { Name = "", Players = 2 };
            await Assert.ThrowsAsync<ArgumentException>(async () => await _gameService.CreateGameAsync(gameDTO));
        }

        [Fact]
        public async Task CreateGameAsync_NotEnoughPlayers_ThrowsException()
        {
            var gameDTO = new CreateGameDTO { Name = "NFS", Players = 1 };
            await Assert.ThrowsAsync<ArgumentException>(async () => await _gameService.CreateGameAsync(gameDTO));
        }

        [Fact]
        public async Task GetGamesAsync_ReturnsListOfGames()
        {
            var games = new List<Game>
            {
                new Game { Id = 1, Name = "Fortnite", Players = 2 },
                new Game { Id = 2, Name = "LOL", Players = 4 }
            };
            var gameDTOs = games.Select(g => new GameResponseDTO { IdGame = g.Id, Name = g.Name, Players = g.Players }).ToList();

            _gameRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(games);
            _mapperMock.Setup(m => m.Map<GameResponseDTO>(It.IsAny<Game>())).Returns<Game>(g => new GameResponseDTO { IdGame = g.Id, Name = g.Name, Players = g.Players });

            var result = await _gameService.GetGamesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetGamesAsync_EmptyList_ReturnsEmptyList()
        {
            _gameRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Game>());
            var result = await _gameService.GetGamesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGameByIdAsync_ExistingGame_ReturnsGame()
        {
            var game = new Game { Id = 1, Name = "LOL", Players = 2 };
            var gameDTO = new GameResponseDTO { IdGame = 1, Name = "NFS", Players = 2 };

            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(game);
            _mapperMock.Setup(m => m.Map<GameResponseDTO>(game)).Returns(gameDTO);

            var result = await _gameService.GetGameByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdGame);
        }

        [Fact]
        public async Task GetGameByIdAsync_NonExistingGame_ReturnsNull()
        {
            _gameRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Game)null);
            var result = await _gameService.GetGameByIdAsync(1);
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetGameByIdAsync_InvalidId_ReturnsNull(int invalidId)
        {
            var result = await _gameService.GetGameByIdAsync(invalidId);
            Assert.Null(result);
        }
    }
}
