using AutoMapper;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Mapping
{
    public class Mapper: Profile
    {
        public Mapper() { 
            CreateMap<Tournament, TournamentResponseDTO>().ReverseMap();
            CreateMap<Tournament, CreateTournamentRequest>().ReverseMap().ForMember(dest => dest.IdOrganizer, opt => opt.MapFrom(src => src.CreatedBy));
            CreateMap<Game, CreateGameDTO>().ReverseMap();
            CreateMap<Game,  GameResponseDTO>().ReverseMap();
            CreateMap<Category, CategoryResponseDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
        }
    }
}
