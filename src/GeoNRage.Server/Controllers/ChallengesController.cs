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
    public partial class ChallengesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ChallengeService _challengeService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<ChallengeDto>> GetAllAsync()
        {
            IEnumerable<Challenge> challenges = await _challengeService.GetAllAsync();

            return _mapper.Map<IEnumerable<Challenge>, IEnumerable<ChallengeDto>>(challenges);
        }
    }
}
