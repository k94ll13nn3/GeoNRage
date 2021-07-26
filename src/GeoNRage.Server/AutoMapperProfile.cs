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
                    src.Challenges.SelectMany(c => c.PlayerScores).DistinctBy(p => p.Player.Id).Select(p => p.Player) :
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
                .ForMember(dest => dest.Rounds, opt => opt.MapFrom(src => src.PlayerGuesses.Select(p => p.Score)))
                .ForMember(dest => dest.GameDate, opt => opt.MapFrom(src => src.Challenge.Game.Date))
                .ForMember(dest => dest.GameId, opt => opt.MapFrom(src => src.Challenge.Game.Id))
                .ForMember(dest => dest.ChallengeId, opt => opt.MapFrom(src => src.Challenge.Id))
                .ForMember(dest => dest.ChallengeTimeLimit, opt => opt.MapFrom(src => src.Challenge.TimeLimit))
                .ForMember(dest => dest.MapName, opt => opt.MapFrom(src => src.Challenge.Map.Name))
                .ForMember(dest => dest.MapId, opt => opt.MapFrom(src => src.Challenge.Map.Id))
                .ForMember(dest => dest.MapIsMapForGame, opt => opt.MapFrom(src => src.Challenge.Map.IsMapForGame));

            CreateMap<Location, LocationDto>();
        }
    }
}
