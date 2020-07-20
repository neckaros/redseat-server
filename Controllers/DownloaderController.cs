using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using redseat_server.Models;
using Microsoft.EntityFrameworkCore;
using redseat_server.Downloaders;

namespace redseat_server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloaderController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DownloaderController> _logger;
        private readonly RedseatDbContext _context;
        private readonly IDownloaderService _downloaderService;

        public DownloaderController(ILogger<DownloaderController> logger, RedseatDbContext context, IDownloaderService downloaderService)
        {
            _logger = logger;
            _context = context;
            _downloaderService = downloaderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Downloader>>>  Get()
        {
            return await _context.Downloaders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Downloader>> GetDownloadById(int id)
        {
            var downloader = await _context.Downloaders.FindAsync(id);

            if (downloader == null)
            {
                return NotFound();
            }

            return downloader;
        }

        [HttpPost]
        public async Task<ActionResult<Download>> CreateDownloaderItem(Downloader downloader)
        {
            await _downloaderService.ValidateDownloader(downloader);
            _context.Downloaders.Add(downloader);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDownloadById),
                new { id = downloader.DownloaderId },
                downloader);
        }
    }
}
