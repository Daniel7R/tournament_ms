using AutoMapper;
using TournamentMS.Application.DTOs.Request;
using TournamentMS.Application.DTOs.Response;
using TournamentMS.Domain.Entities;

namespace TournamentMS.Application.Mapping
{
    public class Mapper: Profile
    {
        public Mapper() {
            CreateMap<Prizes, CreatePrizeDTO>().ReverseMap();
            CreateMap<Tournament, TournamentResponseDTO>().ReverseMap();
            CreateMap<Tournament, FullTournamentResponse>().ReverseMap();
            CreateMap<Tournament, CreateTournamentRequest>().ReverseMap().ForMember(dest => dest.IdOrganizer, opt => opt.MapFrom(src => src.CreatedBy));
            CreateMap<Game, CreateGameDTO>().ReverseMap();
            CreateMap<Game,  GameResponseDTO>().ReverseMap();
            CreateMap<Category, CategoryResponseDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Matches, CreateMatchesRequestDTO>().ReverseMap();
            CreateMap<MatchesResponseDTO, Matches>().ReverseMap().ForMember(dest => dest.IdMatch, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.MatchStatus, opt => opt.MapFrom(src => src.Status));
        }
    }
}
