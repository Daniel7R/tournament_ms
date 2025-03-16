
using Moq;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{


    public class TeamsServiceTests
    {
        private readonly Mock<ITeamsRepository> _teamsRepoMock;
        private readonly Mock<IRepository<TeamsMembers>> _teamsMembersRepoMock;
        private readonly Mock<IEventBusProducer> _eventBusProducerMock;
        private readonly TeamsService _teamsService;

        public TeamsServiceTests()
        {
            _teamsRepoMock = new Mock<ITeamsRepository>();
            _teamsMembersRepoMock = new Mock<IRepository<TeamsMembers>>();
            _eventBusProducerMock = new Mock<IEventBusProducer>();
            _teamsService = new TeamsService(_teamsRepoMock.Object, _teamsMembersRepoMock.Object, _eventBusProducerMock.Object);
        }
        [Fact]
        public async Task GenerateTeams_ShouldCallAddMultipleTeams()
        {
            // Arrange
            var game = new Game { MaxTeams = 4, MaxPlayersPerTeam = 5 };
            int idTournament = 1;

            // Act
            await _teamsService.GenerateTeams(game, idTournament);

            // Assert
            _teamsRepoMock.Verify(repo => repo.AddMultipleTeams(It.IsAny<List<Teams>>()), Times.Once);
        }

        [Fact]
        public async Task AssignTeamMember_WhenUserAlreadyHasTeam_ShouldThrowException()
        {
            // Arrange
            var request = new AssignTeamMemberRequest { IdUser = 1, IdTournament = 2 };
            _teamsRepoMock.Setup(repo => repo.UserHasAlreadyTeam(request.IdUser, request.IdTournament)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _teamsService.AssignTeamMember(request));
        }

        [Fact]
        public async Task AssignTeamMember_WhenNoAvailableTeam_ShouldThrowException()
        {
            // Arrange
            var request = new AssignTeamMemberRequest { IdUser = 1, IdTournament = 2 };
            _teamsRepoMock.Setup(repo => repo.UserHasAlreadyTeam(request.IdUser, request.IdTournament)).ReturnsAsync(false);
            _teamsRepoMock.Setup(repo => repo.GetLowerMembersTeamByIdTournament(request.IdTournament)).ReturnsAsync((Teams)null);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _teamsService.AssignTeamMember(request));
        }

        [Fact]
        public async Task AssignTeamMember_ShouldAddMemberAndUpdateTeam()
        {
            // Arrange
            var request = new AssignTeamMemberRequest { IdUser = 1, IdTournament = 2 };
            var team = new Teams { Id = 10, CurrentMembers = 2, MaxMembers = 5, IsFull = false };

            _teamsRepoMock.Setup(repo => repo.UserHasAlreadyTeam(request.IdUser, request.IdTournament)).ReturnsAsync(false);
            _teamsRepoMock.Setup(repo => repo.GetLowerMembersTeamByIdTournament(request.IdTournament)).ReturnsAsync(team);
            _teamsMembersRepoMock.Setup(repo => repo.AddAsync(It.IsAny<TeamsMembers>())).ReturnsAsync(new TeamsMembers());

            // Act
            await _teamsService.AssignTeamMember(request);

            // Assert
            _teamsMembersRepoMock.Verify(repo => repo.AddAsync(It.IsAny<TeamsMembers>()), Times.Once);
            _teamsRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<Teams>()), Times.Once);
        }

        [Fact]
        public async Task GetFullInformationTeams_ShouldReturnTeamsWithUsersInfo()
        {
            // Arrange
            int idTournament = 1;
            var teams = new List<Teams>
        {
            new Teams
            {
                Id = 1,
                IdTournament = idTournament,
                Name = "Team A",
                CurrentMembers = 2,
                MaxMembers = 5,
                Members = new List<TeamsMembers> { new TeamsMembers { IdUser = 1 } }
            }
        };

            var usersInfo = new List<GetUserByIdResponse>
        {
            new GetUserByIdResponse { Id = 1, Name = "User1", Email = "user1@example.com" }
        };

            _teamsRepoMock.Setup(repo => repo.GetFullInfoTeams(idTournament)).ReturnsAsync(teams);
            _eventBusProducerMock.Setup(producer => producer.SendRequest<List<int>, List<GetUserByIdResponse>>(It.IsAny<List<int>>(), It.IsAny<string>()))
                .ReturnsAsync(usersInfo);

            // Act
            var result = await _teamsService.GetFullInformationTeams(idTournament);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Team A", result.First().TeamName);
            Assert.Equal("User1", result.First().Members.First().Username);
        }
    }


}