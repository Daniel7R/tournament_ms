using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;

namespace TournamentMS.Application.Interfaces
{
    public interface ITournamentService
    {
        Task<IEnumerable<TournamentResponseDTO>> GetTournamentsAsync();
        Task<IEnumerable<FullTournamentResponse>> GetTournamentsByStatus(List<TournamentStatus> statuses);
        Task<TournamentResponseDTO?> GetTournamentByIdAsync(int idTournament);
        Task<TournamentResponseDTO> CreateTournamentAsync(CreateTournamentRequest tournamentDTO, int idUser);
        Task<CreatePrizeDTO> CreatePrizeAndAssignToTournament(CreatePrizeDTO prize, int idTournament, int idUser);
        Task<bool> ChangeTournamentDate(int idUser, int idTournament, ChangeDatesRequest dates);
        Task<bool> UpdateTournamentStatus(ChangeTournamentStatus tournamentStatus, int idTournament, int idUser);
        //Task UpdateTournament(TournamentCreatedDTO tournamentDTO, int idTournament);
        //Task DeleteTournament(int idTournament);
    }
}
