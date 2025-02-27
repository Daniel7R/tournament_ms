using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;

namespace TournamentMS.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetCategoriesAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(int idCategory);
        Task<CategoryResponseDTO> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
    }
}
