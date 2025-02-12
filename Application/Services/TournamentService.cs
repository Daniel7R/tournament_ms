using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TournamentMS.Application.DTO;
using TournamentMS.Application.Interfaces;
using TournamentMS.Domain.Entities;
using TournamentMS.Domain.Enums;
using TournamentMS.Infrastructure.Data;
using TournamentMS.Infrastructure.Repository;

namespace TournamentMS.Application.Service
{
    public class TournamentService : ITournamentService
    {
        //private readonly IRepository<Tournament> _tournamentRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Game> _gameRepository;

        private readonly IMapper _mapper;
             
        //public TournamentService(IRepository<Tournament> tournamentRepository, IRepository<Game> gameRepository, IRepository<Category> categoryRepository, IMapper mapper)
        public TournamentService(ITournamentRepository tournamentRepository, IRepository<Game> gameRepository, IRepository<Category> categoryRepository, IMapper mapper)
        {
            _tournamentRepository = tournamentRepository;
            _categoryRepository = categoryRepository;
            _gameRepository = gameRepository;
            _mapper = mapper;
        }
        public async Task<Tournament> CreateTournamentAsync(CreateTournamentRequest tournamentDTO)
        {
            if (tournamentDTO.Name.IsNullOrEmpty())
            {
                throw new ArgumentException("Tournament name is required");
            }

            if (tournamentDTO.StartDate >= tournamentDTO.EndDate)
                throw new ArgumentException("StartDate must be before EndDate");

            var categoryExists = await _categoryRepository.GetByIdAsync(tournamentDTO.IdCategory);
            if (categoryExists == null)
                throw new ArgumentException("Category does not exist");
            var gameExists = await _gameRepository.GetByIdAsync(tournamentDTO.IdGame);
            if (gameExists == null)
                throw new ArgumentException("Game does not exits");
            
            var tournament = _mapper.Map<Tournament>(tournamentDTO);
                
                /*new Tournament
            {
                Name = tournamentDTO.Name,
                IdCategory = tournamentDTO.IdCategory,
                IdGame = tournamentDTO.IdGame,
                MaxPlayers = tournamentDTO.MaxPlayers,
                IsPaid = tournamentDTO.IsPaid,
                Price = tournamentDTO.Price,
                CreatedBy = tournamentDTO.CreatedBy,
                CreatedAt = DateTime.Now,
                StartDate = tournamentDTO.StartDate,
                EndDate = tournamentDTO.EndDate,
                Status = TournamentStatus.PENDING
            };*/

            

            await _tournamentRepository.AddAsync(tournament);

            return tournament;
        }

        public async Task<TournamentResponseDTO> GetTournamentByIdAsync(int idTournament)
        {
            //var tournament = await _tournamentRepository.GetByIdAsync(idTournament);
            var tournament = await _tournamentRepository.GetTournamentWithCategoriesAndGamesById(idTournament);

            if (tournament == null) return null;
            var tournamentResponse = _mapper.Map<TournamentResponseDTO>(tournament);
                
                /*new TournamentResponseDTO
            {
                Id = tournament.Id,
                Name = tournament.Name,
                CategoryName = tournament.Category.Name,
                GameName = tournament.Game.Name,
                MaxPlayers = tournament.MaxPlayers,
                IsPaid = tournament.IsPaid,
                Price = tournament.Price,
                CreatedAt= tournament.CreatedAt,
                StartDate = tournament.StartDate,
                EndDate = tournament.EndDate
            }; */
            return tournamentResponse;
        }

        public async Task<IEnumerable<TournamentResponseDTO>> GetTournamentsAsync()
        {
            var tournaments = await _tournamentRepository.GetAllAsync();

            var tournamentsResponse = tournaments.Select(t => _mapper.Map<TournamentResponseDTO>(t)).ToList();
                
                /*tournaments.Select(t => new TournamentResponseDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    CategoryName = t.Category.Name,
                    GameName = t.Game.Name,
                    MaxPlayers = t.MaxPlayers,
                    IsPaid = t.IsPaid,
                    Price = t.Price,
                    CreatedAt = t.CreatedAt, 
                    EndDate = t.EndDate,
                    StartDate= t.StartDate,
                }).ToList();*/

            return tournamentsResponse;
        }
        /*
        public async Task UpdateTournament(TournamentCreatedDTO tournamentDTO, int idTournament)
        {
            var tournamentToUpdate = await _db.Tournaments.Select(t => t).Where(t => t.Id == idTournament).FirstOrDefaultAsync();

            tournamentToUpdate.Name = tournamentDTO.Name;
            tournamentToUpdate.IdCategory = tournamentDTO.IdCategory;
            tournamentToUpdate.IdGame = tournamentDTO.IdGame;
            tournamentToUpdate.MaxPlayers = tournamentDTO.MaxPlayers;
            tournamentToUpdate.IsPaid = tournamentDTO.IsPaid;
            tournamentDTO.Price = tournamentDTO.Price;

            await _db.SaveChangesAsync();
        }


        public async Task DeleteTournament(int idTournament)
        {
            var tournamentToDelete = await _db.Tournaments.FindAsync(idTournament);

            _db.Tournaments.Remove(tournamentToDelete);
        }*/
    }
}
