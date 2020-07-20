using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using redseat_server.Models;
using Microsoft.EntityFrameworkCore;

namespace redseat_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DownloadController> _logger;
        private readonly RedseatDbContext _context;

        public DownloadController(ILogger<DownloadController> logger, RedseatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Download>>>  Get()
        {
            return await _context.Downloads.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Download>> GetDownloadById(int id)
        {
            var download = await _context.Downloads.FindAsync(id);

            if (download == null)
            {
                return NotFound();
            }

            return download;
        }

        [HttpPost]
        public async Task<ActionResult<Download>> CreateTodoItem(Download download)
        {

            _context.Downloads.Add(download);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDownloadById),
                new { id = download.DownloadId },
                download);
        }
    }
}