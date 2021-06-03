using System.Collections.Generic;
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

            return _mapper.Map<IEnumerable<Location>, IEnumerable<LocationDto>>(locations);
        }
    }
}
