﻿using System.Threading.Tasks;
using GeoNRage.Server.Services;
using GeoNRage.Shared.Dtos.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoNRage.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [AutoConstructor]
    public partial class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        [HttpGet("info")]
        public async Task<AdminInfoDto> GetAdminInfoAsync()
        {
            return await _adminService.GetAdminInfoAsync();
        }
    }
}