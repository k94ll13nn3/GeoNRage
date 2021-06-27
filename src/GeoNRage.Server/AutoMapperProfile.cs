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
                .ForMember(dest => dest.LocationsCount, opt => opt.MapFrom(src => src.Locations == null ? (int?)null : src.Locations.Count))
                .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.Map.Name))
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.Creator == null ? null : src.Creator.Name));

            CreateMap<PlayerScore, PlayerScoreDto>()
                .ForMember(dest => dest.PlayerName, opt => opt.MapFrom(src => src.Player.Name));
            CreateMap<PlayerScore, PlayerScoreWithChallengeDto>()
                .ForMember(dest => dest.Rounds, opt => opt.MapFrom(src => new[] { src.Round1, src.Round2, src.Round3, src.Round4, src.Round5 }))
                .ForMember(dest => dest.GameDate, opt => opt.MapFrom(src => src.Challenge.Game.Date))
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Challenge.Game.Id))
                .ForMember(dest => dest.ChallengeId, opt => opt.MapFrom(src => src.Challenge.Id))
                .ForMember(dest => dest.ChallengeTimeLimit, opt => opt.MapFrom(src => src.Challenge.TimeLimit))
                .ForMember(dest => dest.MapId, opt => opt.MapFrom(src => src.Challenge.Map.Id));

            CreateMap<Location, LocationDto>();
        }
    }
}
