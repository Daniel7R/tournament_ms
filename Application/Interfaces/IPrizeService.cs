using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Interfaces
{
    public interface IPrizeService
    {
        Task<Prizes> CreatePrize(Prizes prizes);
    }
}
