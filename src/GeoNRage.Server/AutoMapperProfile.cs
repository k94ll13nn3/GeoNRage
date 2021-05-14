using System.Linq;
using AutoMapper;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;

namespace GeoNRage.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Map, MapDto>();
            CreateMap<Player, PlayerDto>();
            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Challenges.Count > 0 ? src.Challenges.First().PlayerScores.Select(p => p.Player) : Enumerable.Empty<Player>()));
            CreateMap<Game, GameLightDto>();
            CreateMap<Challenge, ChallengeDto>()
                .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.Map.Name));
            CreateMap<PlayerScore, PlayerScoreDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Name));
        }
    }
}
