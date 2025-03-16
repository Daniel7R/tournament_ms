using AutoMapper;
using Moq;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class MatchesServiceTests
    {
        private readonly Mock<IMatchesRepository> _matchesRepoMock;
        private readonly Mock<ITournamentRepository> _tournamentRepoMock;
        private readonly Mock<ITeamsService> _teamsServiceMock;
        private readonly Mock<ITournamentUserRoleRepository> _userTournamentRoleMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEventBusProducer> _eventBusMock;
        private readonly MatchesService _matchesService;

        public MatchesServiceTests()
        {
            _matchesRepoMock = new Mock<IMatchesRepository>();
            _tournamentRepoMock = new Mock<ITournamentRepository>();
            _teamsServiceMock = new Mock<ITeamsService>();
            _userTournamentRoleMock = new Mock<ITournamentUserRoleRepository>();
            _mapperMock = new Mock<IMapper>();
            _eventBusMock = new Mock<IEventBusProducer>();

            _matchesService = new MatchesService(
                _matchesRepoMock.Object,
                _teamsServiceMock.Object,
                _userTournamentRoleMock.Object,
                _tournamentRepoMock.Object,
                _mapperMock.Object,
                _eventBusMock.Object
            );
        }

        [Fact]
        public async Task CreateMatch_ShouldThrowException_WhenTournamentDoesNotExist()
        {
            // Arrange
            var request = new CreateMatchesRequestDTO { IdTournament = 1, MatchDate = DateTime.UtcNow };
            _tournamentRepoMock.Setup(repo => repo.GetByIdAsync(request.IdTournament))
                .ReturnsAsync((Tournament)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _matchesService.CreateMatch(request, 1));
        }

        [Fact]
        public async Task CreateMatch_ShouldThrowException_WhenUserHasNoPermissions()
        {
            // Arrange
            var request = new CreateMatchesRequestDTO { IdTournament = 1, MatchDate = DateTime.UtcNow };
            var tournament = new Tournament { Id = 1, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(10) };
            _tournamentRepoMock.Setup(repo => repo.GetByIdAsync(request.IdTournament))
                .ReturnsAsync(tournament);
            _userTournamentRoleMock.Setup(repo => repo.GetUserRole(It.IsAny<int>(), It.IsAny<int>(), EventType.TOURNAMENT))
                .ReturnsAsync((TournamentUserRole)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidRoleException>(() => _matchesService.CreateMatch(request, 1));
        }

        [Fact]
        public async Task GetMatchById_ShouldReturnMatch_WhenMatchExists()
        {
            // Arrange
            var match = new Matches { Id = 1, Name = "Match 1" };
            var responseDto = new MatchesResponseDTO { IdMatch= 1, Name = "Match 1" };

            _matchesRepoMock.Setup(repo => repo.GetMatchbyId(1)).ReturnsAsync(match);
            _mapperMock.Setup(mapper => mapper.Map<MatchesResponseDTO>(match)).Returns(responseDto);

            // Act
            var result = await _matchesService.GetMatchById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(match.Id, result.IdMatch);
        }

        [Fact]
        public async Task SetWinnerMatch_ShouldThrowException_WhenMatchDoesNotExist()
        {
            // Arrange
            var winnerDto = new MatchWinnerDTO { IdMatch = 1, IdWinner = 2 };
            _matchesRepoMock.Setup(repo => repo.GetMatchbyId(1)).ReturnsAsync((Matches)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _matchesService.SetWinnerMatch(winnerDto, 1));
        }
    }

}