using Moq;
using TournamentMS.Application.Services;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
using TournamentMS.Infrastructure.Repository;
using Xunit;

namespace TournamentMS.Tests
{
    public class PrizeServiceTests
    {
        private readonly Mock<IRepository<Prizes>> _prizesRepoMock;
        private readonly PrizeService _prizeService;
        public PrizeServiceTests()
        {
            _prizesRepoMock = new Mock<IRepository<Prizes>>();
            _prizeService = new PrizeService(_prizesRepoMock.Object);
        }

        [Fact]
        public async Task GetPrizeById_ExistingPrize_ReturnsPrize()
        {
            // Arrange
            var prize = new Prizes { Id = 1,   Description= "Gold Medal", Total = 1000 };
            _prizesRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(prize);

            // Act
            var result = await _prizeService.GetPrizeById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Gold Medal", result.Description);
        }

        [Fact]
        public async Task GetPrizeById_NonExistingPrize_ReturnsEmptyPrize()
        {
            // Arrange
            _prizesRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Prizes)null);

            // Act
            var result = await _prizeService.GetPrizeById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id); 
            Assert.Null(result.Description);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetPrizeById_InvalidId_ReturnsEmptyPrize(int invalidId)
        {
            // Act
            var result = await _prizeService.GetPrizeById(invalidId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Id);
            Assert.Null(result.Description);
        }

        [Fact]
        public async Task CreatePrize_ValidPrize_ReturnsCreatedPrize()
        {
            // Arrange
            var prize = new Prizes { Id = 2, Description = "Silver Medal", Total = 500 };
            _prizesRepoMock.Setup(repo => repo.AddAsync(prize)).ReturnsAsync(prize);

            // Act
            var result = await _prizeService.CreatePrize(prize);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Id);
            Assert.Equal("Silver Medal", result.Description);
            _prizesRepoMock.Verify(repo => repo.AddAsync(prize), Times.Once);
        }

        [Fact]
        public async Task CreatePrize_NullPrize_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleException
            >(() => _prizeService.CreatePrize(null));
        }
    }

}