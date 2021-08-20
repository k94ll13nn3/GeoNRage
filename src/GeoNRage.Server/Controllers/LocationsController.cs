using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class LocationsController : ControllerBase
{
    private readonly LocationService _locationService;

    [HttpGet]
    public async Task<IEnumerable<LocationDto>> GetAllAsync()
    {
        return await _locationService.GetAllAsync(Request.Headers["show-all-maps"] == "True");
    }
}
