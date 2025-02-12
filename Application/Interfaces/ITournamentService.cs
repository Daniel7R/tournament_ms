using TournamentMS.Application.DTO;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentResponseDTO>> GetTournamentsAsync();
        Task<TournamentResponseDTO> GetTournamentByIdAsync(int idTournament);
        Task<Tournament> CreateTournamentAsync(CreateTournamentRequest tournamentDTO);
        //Task UpdateTournament(TournamentCreatedDTO tournamentDTO, int idTournament);
        //Task DeleteTournament(int idTournament);
    }
}
