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
            CreateMap<Player, PlayerFullDto>();

            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src =>
                    src.Challenges.Count > 0 ?
                    src.Challenges.SelectMany(c => c.PlayerScores).Select(p => p.Player).Distinct() :
                    Enumerable.Empty<Player>()));
            CreateMap<Game, GameLightDto>();

            CreateMap<Challenge, ChallengeDto>()
                .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Game.Name))
                .ForMember(dest => dest.GameDate, opt => opt.MapFrom(src => src.Game.Date))
                .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.Map.Name));

            CreateMap<PlayerScore, PlayerScoreDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Name));
            CreateMap<PlayerScore, PlayerScoreWithChallengeDto>();
        }
    }
}
