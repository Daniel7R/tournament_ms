using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Services
{
    public class PrizeService : IPrizeService
    {
        private readonly IRepository<Prizes> _prizesRepo;

        public PrizeService(IRepository<Prizes> prizesRepo)
        {
            _prizesRepo = prizesRepo;
        }

        public async Task<Prizes> GetPrizeById(int idPrize)
        {
            var response = await _prizesRepo.GetByIdAsync(idPrize);

            return response ?? new Prizes();
        }
        public async Task<Prizes> CreatePrize(Prizes prizes)
        {
            await _prizesRepo.AddAsync(prizes);
            return prizes;
        }
    }
}
