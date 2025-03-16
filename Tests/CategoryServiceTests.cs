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
    public class CategoryServiceTests
    {
        private readonly Mock<IRepository<Category>> _categoryRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepositoryMock = new Mock<IRepository<Category>>();
            _mapperMock = new Mock<IMapper>();
            _categoryService = new CategoryService(_categoryRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateCategoryAsync_ValidCategory_ReturnsCategoryResponse()
        {
            // Arrange
            var categoryDTO = new CreateCategoryDTO { Code = "C1", Name = "Category 1", Alias = "Cat1" };
            var category = new Category { Id = 1, Code = "C1", Name = "Category 1", Alias = "Cat1" };
            var categoryResponseDTO = new CategoryResponseDTO { IdCategory = 1, Code = "C1", Name = "Category 1", Alias = "Cat1" };

            _mapperMock.Setup(m => m.Map<Category>(categoryDTO)).Returns(category);
            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(category)).Returns(categoryResponseDTO);
            _categoryRepositoryMock.Setup(repo => repo.AddAsync(category)).ReturnsAsync(category);

            // Act
            var result = await _categoryService.CreateCategoryAsync(categoryDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryResponseDTO.IdCategory, result.IdCategory);
        }

        [Fact]
        public async Task CreateCategoryAsync_NullCategoryDTO_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () => await _categoryService.CreateCategoryAsync(null));
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsListOfCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = 1, Code = "C1", Name = "Category 1", Alias = "Cat1" },
                new Category { Id = 2, Code = "C2", Name = "Category 2", Alias = "Cat2" }
            };
            var categoryDTOs = categories.Select(c => new CategoryResponseDTO { IdCategory = c.Id, Code = c.Code, Name = c.Name, Alias = c.Alias }).ToList();

            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(It.IsAny<Category>())).Returns<Category>(c => new CategoryResponseDTO { IdCategory = c.Id, Code = c.Code, Name = c.Name, Alias = c.Alias });

            var result = await _categoryService.GetCategoriesAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ExistingCategory_ReturnsCategory()
        {
            var category = new Category { Id = 1, Code = "C1", Name = "Category 1", Alias = "Cat1" };
            var categoryDTO = new CategoryResponseDTO { IdCategory = 1, Code = "C1", Name = "Category 1", Alias = "Cat1" };

            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(category)).Returns(categoryDTO);

            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.IdCategory);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_NonExistingCategory_ReturnsNull()
        {
            _categoryRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Category)null);

            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetCategoriesAsync_EmptyList_ReturnsEmptyList()
        {
            _categoryRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Category>());
            var result = await _categoryService.GetCategoriesAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetCategoryByIdAsync_InvalidId_ReturnsNull(int invalidId)
        {
            var result = await _categoryService.GetCategoryByIdAsync(invalidId);
            Assert.Null(result);
        }
    }
}