using AutoMapper;
using GeoNRage.Server.Entities;
using GeoNRage.Shared.Dtos;

namespace GeoNRage.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Map, MapDto>()
                .ForMember(dest => dest.GameCount, opt => opt.MapFrom(src => src.Games.Count));

            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.GameCount, opt => opt.MapFrom(src => src.Games.Count));

            CreateMap<Value, ValueDto>();

            CreateMap<Game, GameDto>();
        }
    }
}
