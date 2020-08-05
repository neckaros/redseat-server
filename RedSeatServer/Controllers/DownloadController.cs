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

namespace RedSeatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly ILogger<DownloadController> _logger;
        private readonly RedseatDbContext _context;
        private readonly IMapper _mapper;

        public DownloadController(ILogger<DownloadController> logger, RedseatDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        [EnableCors("LocalAndServer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DownloadDto>>>  Get()
        {
            var downloads =  await _context.Downloads.Include("Files").ToListAsync();
            
            return _mapper.Map<List<DownloadDto>>(downloads);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DownloaderDto>> GetDownloadById(int id)
        {
            var download = await _context.Downloads.FindAsync(id);

            if (download == null)
            {
                return NotFound();
            }

            return _mapper.Map<DownloaderDto>(download);
        }

        // [HttpPost]
        // public async Task<ActionResult<Download>> CreateTodoItem([FromForm] IFormFile file)
        // {
        //     //_logger.LogInformation($"Received download file: {file.Name} ({file.Length})");
        //     // _context.Downloads.Add(download);
        //     // await _context.SaveChangesAsync();
        //         return NotFound();
        //     // return CreatedAtAction(
        //     //     nameof(GetDownloadById),
        //     //     new { id = download.DownloadId },
        //     //     download);
        // }
    }
}
