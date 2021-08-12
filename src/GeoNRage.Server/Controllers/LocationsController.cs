using GeoNRage.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[AutoConstructor]
public partial class LocationsController : ControllerBase
{
    private readonly LocationService _locationService;

    [AllowAnonymous]
    [HttpGet]
    public async Task<IEnumerable<LocationDto>> GetAllAsync()
    {
        return await _locationService.GetAllAsync(Request.Headers["show-all-maps"] == "True");
    }
}
