using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Exceptions;
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
            if (prizes == null)
            {
                throw new BusinessRuleException("The prize cannot be null.");
            }
            await _prizesRepo.AddAsync(prizes);
            return prizes;
        }
    }
}
