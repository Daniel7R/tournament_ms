using Moq;
using TournamentMS.Application.Interfaces;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Queues;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class ReminderServiceTests
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IEventBusProducer> _eventBusProducerMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<ITournamentRepository> _tournamentRepoMock;
        private readonly ReminderService _reminderService;

        public ReminderServiceTests()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _eventBusProducerMock = new Mock<IEventBusProducer>();
            _tournamentRepoMock = new Mock<ITournamentRepository>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();

            _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            _serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_serviceScopeFactoryMock.Object);
            _serviceProviderMock.Setup(x => x.GetService(typeof(ITournamentRepository))).Returns(_tournamentRepoMock.Object);

            _reminderService = new ReminderService(_serviceProviderMock.Object, _eventBusProducerMock.Object);
        }

        [Fact]
        public async Task SendReminder_WithMatches_SendsEmailNotification()
        {
            // Arrange
            var matches = new List<Tournament>
            {
                new Tournament
                {
                    Name = "Fornite Legends",
                    Matches = new List<Matches>
                    {
                        new Matches
                        {
                            Name = "Final Match",
                            Date = DateTime.UtcNow,
                            TeamsMatches = new List<TeamsMatches>
                            {
                                new TeamsMatches { Team = new Teams { Name = "Team A" }},
                                new TeamsMatches { Team = new Teams { Name = "Team B" }}
                            }
                        }
                    }
                }
            };

            _tournamentRepoMock.Setup(repo => repo.GetTournamentsAndMatchesCurrentDay())
                .ReturnsAsync(matches);

            // Act
            await _reminderService.SendReminder();

            // Assert
            _eventBusProducerMock.Verify(x => x.PublishEventAsync(
                It.Is<EmailBulkNotificationRequest>(e => 
                    e.Subject.Contains("🔔 Reminder Today Matches🔔") &&
                    e.Body.Contains("Final Match") &&
                    e.Body.Contains("Team A vs Team B")
                ),
                Queues.REMINDER
            ), Times.Once);
        }

        [Fact]
        public async Task SendReminder_NoMatches_DoesNotSendNotification()
        {
            // Arrange
            _tournamentRepoMock.Setup(repo => repo.GetTournamentsAndMatchesCurrentDay())
                .ReturnsAsync(new List<Tournament>());

            // Act
            await _reminderService.SendReminder();

            // Assert
            _eventBusProducerMock.Verify(x => x.PublishEventAsync<EmailBulkNotificationRequest>(
                It.IsAny<EmailBulkNotificationRequest>(), It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task SendReminder_RepositoryReturnsNull_DoesNotSendNotification()
        {
            // Arrange
            _tournamentRepoMock.Setup(repo => repo.GetTournamentsAndMatchesCurrentDay())
                .ReturnsAsync((List<Tournament>)null);

            // Act
            await _reminderService.SendReminder();

            // Assert
            _eventBusProducerMock.Verify(x => x.PublishEventAsync<EmailBulkNotificationRequest>(
                It.IsAny<EmailBulkNotificationRequest>(), It.IsAny<string>()), 
                Times.Never);
        }
    }
}