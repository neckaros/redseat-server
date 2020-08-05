using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using RedSeatServer.Services;
using Microsoft.AspNetCore.Cors;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;

namespace RedSeatServer.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
                private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly ILogger<UserController> _logger;
        private readonly IAppService _appService;
        private readonly IRsDriveService _rsDriveService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly RedseatDbContext _context;

        public UserController(ILogger<UserController> logger, IAppService appService, IRsDriveService rsDriveService, IHttpClientFactory clientFactory,  IBackgroundJobClient backgroundJobs, RedseatDbContext context)
        {
            _logger = logger;
            _appService = appService;
            _rsDriveService = rsDriveService;
            _clientFactory = clientFactory;
            _backgroundJobs = backgroundJobs;
            _context = context;
        }
        
        [Authorize]
        [EnableCors("LocalAndServer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IpResult>>> Get()
        {
            var ip = await _appService.GetExternalIp().ToListAsync();
            var identity = User.Identity as ClaimsIdentity;
            
            var id = identity.Claims.First(c => c.Type == "user_id");
            return Ok(id.Value);
            //return Ok(claims);
            //return Ok(ip);
        }

        [EnableCors("LocalAndServer")]
        [HttpGet("token")]
        public async Task<ActionResult<IEnumerable<IpResult>>> GetToken(string link, string tokenid, string name, string parent)
        {
            
            var auths = HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationToken);
            var bearer = authorizationToken.FirstOrDefault();
            var key = bearer?.Replace("Bearer ", "");
            
            if (key == null) {
                return Unauthorized("No bearer token provided");
            }

            var id = _backgroundJobs.Enqueue<RsDriveService>((d) => d.upload(0, tokenid, key, name, parent));

         
            return Ok(id);
        }

        
        
        [EnableCors("LocalAndServer")]
        [HttpGet("verify")]
        public ActionResult<JwtSecurityToken> VerifyToken(string token)
        {
            //var decodedToken = _firebaseService.VerifyToken(token);
            return Ok();
        }


    }
}
