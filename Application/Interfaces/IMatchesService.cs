using TournamentMS.Application.DTOs;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface IMatchesService
    {
        Task<MatchesResponseDTO> CreateMatch(CreateMatchesRequestDTO match2Create, int idUser);
        Task<IEnumerable<MatchesResponseDTO>> GetMatchesByIdTournament(int idTournament);
        Task<MatchesResponseDTO> GetMatchById(int idMatch);
        Task<bool> SetWinnerMatch(MatchWinnerDTO matchWinner, int idUser);
        Task<bool> ChangeMatchDate(ChangeMatchhDate changeMatchhDate, int idUser);
    }
}
