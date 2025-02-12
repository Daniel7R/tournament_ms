using AutoMapper;
using TournamentMS.Application.DTO;
using TournamentMS.Application.DTOs;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Mapping
{
    public class Mapper: Profile
    {
        public Mapper() { 
            CreateMap<Tournament, TournamentResponseDTO>().ReverseMap();
            CreateMap<Tournament, CreateTournamentRequest>().ReverseMap();
            CreateMap<Game, CreateGameDTO>().ReverseMap();
            CreateMap<Game,  GameResponseDTO>().ReverseMap();
            CreateMap<Category, CategoryResponseDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
        }
    }
}
