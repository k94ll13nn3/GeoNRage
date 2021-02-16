using AutoMapper;
using GeoNRage.Data.Dtos;
using GeoNRage.Data.Entities;

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
        }
    }
}
