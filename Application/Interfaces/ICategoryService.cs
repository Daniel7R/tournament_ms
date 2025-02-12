using TournamentMS.Application.DTOs;

namespace TournamentMS.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(int idCategory);
        Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
    }
}
