using AutoMapper;
using Moq;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Queues;
using TournamentMS.Application.Service;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class TournamentServiceTests
    {
        private readonly Mock<ITournamentRepository> _mockTournamentRepo;
        private readonly Mock<ITournamentUserRoleRepository> _mockUserRoleRepo;
        private readonly Mock<IRepository<Game>> _mockGameRepo;
        private readonly Mock<IRepository<Category>> _mockCategoryRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IEventBusProducer> _mockEventBus;
        private readonly Mock<IPrizeService> _mockPrizeService;
        private readonly Mock<ITeamsService> _mockTeamsService;

        private readonly TournamentService _tournamentService;

        public TournamentServiceTests()
        {
            _mockTournamentRepo = new Mock<ITournamentRepository>();
            _mockUserRoleRepo = new Mock<ITournamentUserRoleRepository>();
            _mockGameRepo = new Mock<IRepository<Game>>();
            _mockCategoryRepo = new Mock<IRepository<Category>>();
            _mockMapper = new Mock<IMapper>();
            _mockEventBus = new Mock<IEventBusProducer>();
            _mockPrizeService = new Mock<IPrizeService>();
            _mockTeamsService = new Mock<ITeamsService>();

            _tournamentService = new TournamentService(
                _mockTournamentRepo.Object,
                _mockUserRoleRepo.Object,
                _mockGameRepo.Object,
                _mockCategoryRepo.Object,
                _mockMapper.Object,
                _mockEventBus.Object,
                _mockPrizeService.Object,
                _mockTeamsService.Object
            );
        }
        // [Fact]
        // public async Task CreateTournamentAsync_Should_Create_Tournament_Successfully()
        // {
        //     // Arrange
        //     var tournamentRequest = new CreateTournamentRequest
        //     {
        //         Name = "Test Tournament",
        //         IdCategory = 1,
        //         IdGame = 1,
        //         StartDate = DateTime.UtcNow.AddDays(1),
        //         EndDate = DateTime.UtcNow.AddDays(5),
        //         CreatedBy = 1,
        //         IsFree = false,
        //         Prize = new CreatePrizeDTO { Total = 1000, Description = "First Prize" }
        //     };

        //     var fakePrize = new Prizes{
        //         Total= 1000,
        //         Description= "First Prize",
        //         Id= 1
        //     };
        //     var category = new Category { Id = 1, Name = "Esports", LimitParticipant = 10 };
        //     var game = new Game { Id = 1, Name = "FIFA", Players = 2 };
        //     var tournament = new Tournament { Id = 1, Name = "Test Tournament", IdPrize = 1 };

        //     _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(tournamentRequest.IdCategory))
        //         .ReturnsAsync(category);
        //     _mockGameRepo.Setup(repo => repo.GetByIdAsync(tournamentRequest.IdGame))
        //         .ReturnsAsync(game);
        //     _mockTournamentRepo.Setup(repo => repo.AddAsync(It.IsAny<Tournament>()))
        //         .ReturnsAsync(tournament);
        //     _mockMapper.Setup(m => m.Map<Tournament>(It.IsAny<CreateTournamentRequest>()))
        //         .Returns(tournament);
        //     _mockMapper.Setup(m => m.Map<TournamentResponseDTO>(It.IsAny<Tournament>()))
        //         .Returns(new TournamentResponseDTO { Id = 1, Name = "Test Tournament", MaxPlayers = 2 });
        //     _mockPrizeService.Setup(m => m.CreatePrize(new Prizes{ Total=1000, Description="First Prize"})).ReturnsAsync(fakePrize);

        //     // Act 
        //     var result = await _tournamentService.CreateTournamentAsync(tournamentRequest, 1);

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.Equal("Test Tournament", result.Name);
        //     Assert.Equal(2, result.MaxPlayers);

        //     _mockTournamentRepo.Verify(repo => repo.AddAsync(It.IsAny<Tournament>()), Times.Once);
        //     _mockTeamsService.Verify(service => service.GenerateTeams(game, tournament.Id), Times.Once);
        //     _mockEventBus.Verify(bus => bus.PublishEventAsync<EmailBulkNotificationRequest>(
        //         It.IsAny<EmailBulkNotificationRequest>(),
        //         Queues.SEND_EMAIL_CREATE_TOURNAMENT),
        //         Times.Once);
        // }

        [Fact]
        public async Task CreateTournamentAsync_Should_Throw_Exception_When_StartDate_In_Past()
        {
            // Arrange
            var tournamentRequest = new CreateTournamentRequest
            {
                Name = "Invalid Tournament",
                IdCategory = 1,
                IdGame = 1,
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(5),
                CreatedBy = 1,
                IsFree = false
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _tournamentService.CreateTournamentAsync(tournamentRequest, 1));
        }

        [Fact]
        public async Task CreateTournamentAsync_Should_Throw_Exception_When_Category_Not_Found()
        {
            // Arrange
            var tournamentRequest = new CreateTournamentRequest
            {
                Name = "Tournament Without Category",
                IdCategory = 999,
                IdGame = 1,
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(5),
                CreatedBy = 1,
                IsFree = false
            };

            _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(tournamentRequest.IdCategory))
                .ReturnsAsync((Category)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _tournamentService.CreateTournamentAsync(tournamentRequest, 1));
        }
    }
}