namespace TournamentMS.Application.Interfaces
{
    public interface ITournamentValidations
    {
        Task<bool> UserHasAlreadyFreeTournaments(int idUser);
    }
}
