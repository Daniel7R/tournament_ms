using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
    
        public async Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            if (categoryDTO.Code.IsNullOrEmpty()) throw new ArgumentException("Code is required");
            if(categoryDTO.Name.IsNullOrEmpty()) throw new ArgumentException("Name is required");
            if (categoryDTO.Alias.IsNullOrEmpty()) throw new ArgumentException("Alias is required");

            var category = _mapper.Map<Category>(categoryDTO);

            await _categoryRepository.AddAsync(category);

            var categoryResponse = _mapper.Map<CategoryResponseDTO>(category);

            return categoryResponse;

        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var categoriesResponse = categories.Select(c => _mapper.Map<CategoryResponseDTO>(c)).ToList();

            return categoriesResponse;
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(int idCategory)
        {
            var category = await _categoryRepository.GetByIdAsync(idCategory);
            if (category == null) return null;

            var categoryResponse = _mapper.Map<CategoryResponseDTO>(category);

            return categoryResponse;
        }
    }
}
