using Moq;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class UserTournamentRoleServiceTests
    {
        private readonly Mock<ILogger<UserTournamentRoleService>> _mockLogger;
        private readonly Mock<ITournamentUserRoleRepository> _mockRepo;
        private readonly UserTournamentRoleService _service;

        public UserTournamentRoleServiceTests()
        {
            _mockLogger = new Mock<ILogger<UserTournamentRoleService>>();
            _mockRepo = new Mock<ITournamentUserRoleRepository>();
            _service = new UserTournamentRoleService(_mockLogger.Object, _mockRepo.Object);
        }

        [Fact]
        public async Task AssignRoleUser_ShouldThrowException_WhenUserIsNotAdmin()
        {
            // Arrange
            _mockRepo
                .Setup(repo => repo.GetUserRole(It.IsAny<int>(), It.IsAny<int>(), EventType.TOURNAMENT))
                .ReturnsAsync((TournamentUserRole)null);

            var createUserRoleDTO = new CreateUserRoleDTO
            {
                EventType = EventType.TOURNAMENT,
                IdEvent = 1,
                IdUser = 2,
                Role = TournamentRoles.SUBADMIN
            };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _service.AssignRoleUser(createUserRoleDTO, 3));
        }

        [Fact]
        public async Task AssignRoleUser_ShouldThrowException_WhenMaxSubAdminsReached()
        {
            // Arrange
            var tournamentId = 1;
            var existingSubadmins = new List<TournamentUserRole>
            {
                new TournamentUserRole { IdTournament = tournamentId, Role = TournamentRoles.SUBADMIN },
                new TournamentUserRole { IdTournament = tournamentId, Role = TournamentRoles.SUBADMIN }
            };

            _mockRepo.Setup(repo => repo.GetUserRole(It.IsAny<int>(), tournamentId, EventType.TOURNAMENT))
                    .ReturnsAsync(new TournamentUserRole { Role = TournamentRoles.ADMIN });

            _mockRepo.Setup(repo => repo.GetByIdTournament(tournamentId))
                    .ReturnsAsync(existingSubadmins);

            var createUserRoleDTO = new CreateUserRoleDTO
            {
                EventType = EventType.TOURNAMENT,
                IdEvent = tournamentId,
                IdUser = 2,
                Role = TournamentRoles.SUBADMIN
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.AssignRoleUser(createUserRoleDTO, 3));
        }
    }
}