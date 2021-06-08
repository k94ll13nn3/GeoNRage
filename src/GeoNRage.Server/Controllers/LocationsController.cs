using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GeoNRage.Server.Entities;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [AutoConstructor]
    public partial class LocationsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly LocationService _locationService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<LocationDto>> GetAllAsync()
        {
            IEnumerable<Location> locations = await _locationService.GetAllAsync();

            var results = new List<LocationDto>();
            foreach (IGrouping<(decimal Latitude, decimal Longitude), Location> location in locations.GroupBy(l => (l.Latitude, l.Longitude)))
            {
                LocationDto dto = _mapper.Map<Location, LocationDto>(location.First());
                dto.TimesSeen = location.Count();
                results.Add(dto);
            }
            return results;
        }
    }
}
