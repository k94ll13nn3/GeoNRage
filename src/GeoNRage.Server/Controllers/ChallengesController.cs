using System;
using System.Collections.Generic;
using System.Net.Http;
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

        [AllowAnonymous]
        [HttpGet("without-games")]
        public async Task<IEnumerable<ChallengeDto>> GetAllWithoutGameAsync()
        {
            IEnumerable<Challenge> challenges = await _challengeService.GetAllWithoutGameAsync();

            return _mapper.Map<IEnumerable<Challenge>, IEnumerable<ChallengeDto>>(challenges);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<ChallengeDto>> GetAsync(int id)
        {
            Challenge? challenge = await _challengeService.GetAsync(id);
            if (challenge == null)
            {
                return NotFound();
            }

            return _mapper.Map<Challenge, ChallengeDto>(challenge);
        }

        [AllowAnonymous]
        [HttpPost("import")]
        public async Task<ActionResult<int>> ImportChallengeAsync(ChallengeImportDto dto)
        {
            _ = dto ?? throw new ArgumentNullException(nameof(dto));

            try
            {
                return await _challengeService.ImportChallengeAsync(dto);
            }
            catch (HttpRequestException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _challengeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
