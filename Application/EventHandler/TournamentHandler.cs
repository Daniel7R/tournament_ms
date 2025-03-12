using Newtonsoft.Json;
using TournamentMS.Application.Messages.Request;
using TournamentMS.Application.Messages.Response;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.EventHandler
{
    public class TournamentHandler
    {
        private readonly ILogger<TournamentHandler> _logger;
        private readonly ITournamentRepository _repository;
        private readonly IMatchesRepository _matchesRepository;

        public TournamentHandler(ILogger<TournamentHandler> logger, ITournamentRepository repository, IMatchesRepository matchesRepository)
        {
            _logger = logger;
            _repository = repository;
            _matchesRepository = matchesRepository;
        }

        public async Task<IEnumerable<GetTournamentBulkResponse>> GetTournamentsByIds(List<int> ids)
        {
            _logger.LogInformation($"gettings tournaments info");

             var tournaments =   await _repository.GetTournamentsByIds(ids);
            IEnumerable<GetTournamentBulkResponse> getTournaments = tournaments.Select(x => new GetTournamentBulkResponse
            {
                Id= x.Id,
                Name= x.Name
            });
            
            if(tournaments.Count()==0) getTournaments= new List<GetTournamentBulkResponse>();

            return getTournaments;
        }

        public async Task<bool?> IsFreeTournament(int idMatch)
        {
            _logger.LogInformation($"Is free for match {idMatch}");

            var match = await _matchesRepository.GetMatchbyId(idMatch);
            //
            if(match == null)
            {
                return null;
            }
            _logger.LogInformation($"Match: {JsonConvert.SerializeObject(match)}");
            var tournament = await _repository.GetTournamentWithCategoriesAndGamesById(match.IdTournament);
            _logger.LogInformation($"");

            return tournament?.IsFree;
        }

        public async Task<bool> MatchBelongsTournament(ValidateMatchTournament request)
        {
            _logger.LogInformation($"reques to validate if match belongs to tournament");
            var match = await _matchesRepository.GetMatchbyId(request.IdMatch);

            if(match != null && match.IdTournament == request.IdTournament)
            {
                return true;
            }

            return false;
        }
    }
}
