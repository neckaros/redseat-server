using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedSeatServer.Models;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using Open.Nat;
using RedSeatServer.Services;
using Hangfire;

namespace RedSeatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly RedseatDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRsDriveService _rsDriveService;
        private readonly IBackgroundJobClient _backgroundJobs;

        public TokenController(ILogger<TokenController> logger, RedseatDbContext context, IMapper mapper, IRsDriveService rsDriveService, IBackgroundJobClient backgroundJobs)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
            _rsDriveService = rsDriveService;
            _backgroundJobs = backgroundJobs;
        }
        [EnableCors("LocalAndServer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TokenDto>>> Get()
        {
            var tokens = await _context.Tokens.AsQueryable().ToListAsync();

            return _mapper.Map<List<TokenDto>>(tokens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TokenDto>> GetDownloadById(string id)
        {
            var token = await _context.Tokens.FindAsync(id);

            if (token == null)
            {
                return NotFound();
            }

            return _mapper.Map<TokenDto>(token);
        }

        [EnableCors("LocalAndServer")]
        [HttpPost("{token}")]
        public async Task<ActionResult<IEnumerable<String>>> addUpload(int file, string token, string name, string parent)
        {

            var tokenRetreived = await _context.Tokens.FindAsync(token);
            if (tokenRetreived == null)
            {
                return NotFound("token not found)");
            }

            var fileRetreived = await _context.Files.FindAsync(file);
            if (fileRetreived == null)
            {
                return NotFound("file not found)");
            }


            var id = _backgroundJobs.Enqueue<RsDriveService>((d) => d.upload(file, tokenRetreived.Id, tokenRetreived.Key, name, parent));
            //await _rsDriveService.upload(file, tokenRetreived.Id, tokenRetreived.Key, name, parent);

            return Ok(id);
        }


        [EnableCors("LocalAndServer")]
        [HttpGet("{token}/browse")]
        public async Task<ActionResult<IEnumerable<DriveFile>>> browse(string parent, string token)
        {

            var tokenRetreived = await _context.Tokens.FindAsync(token);
            if (tokenRetreived == null)
            {
                return NotFound("token not found)");
            }

            var files = await _rsDriveService.Browse(parent, tokenRetreived.Id, tokenRetreived.Key);

            return Ok(files);
        }

        [HttpPost]
        public async Task<ActionResult<Download>> AddToken([FromBody] Token token)
        {
            var savedToken = _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            return Ok(savedToken);
        }
    }
}
