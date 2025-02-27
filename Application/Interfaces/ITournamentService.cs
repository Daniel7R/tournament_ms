using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentResponseDTO>> GetTournamentsAsync();
        Task<TournamentResponseDTO?> GetTournamentByIdAsync(int idTournament);
        Task<TournamentResponseDTO> CreateTournamentAsync(CreateTournamentRequest tournamentDTO);
        //Task UpdateTournament(TournamentCreatedDTO tournamentDTO, int idTournament);
        //Task DeleteTournament(int idTournament);
    }
}
