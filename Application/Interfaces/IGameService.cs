using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface IGameService
    {
        Task<IEnumerable<GameResponseDTO>> GetGamesAsync();
        Task<GameResponseDTO> GetGameByIdAsync(int idGame);
        Task<GameResponseDTO> CreateGameAsync(CreateGameDTO gameDTO);
    }
}
