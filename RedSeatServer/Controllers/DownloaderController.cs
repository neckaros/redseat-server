using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedSeatServer.Models;
using RedSeatServer.Downloaders;
using Microsoft.AspNetCore.Http;
using RedSeatServer.Services;
using System.ComponentModel.DataAnnotations;

namespace RedSeatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloaderController : ControllerBase
    {
        private readonly ILogger<DownloaderController> _logger;
        private readonly RedseatDbContext _context;
        private readonly IDownloadersService _downloadersService;

        public DownloaderController(ILogger<DownloaderController> logger, RedseatDbContext context, IDownloadersService downloadersService)
        {
            _logger = logger;
            _context = context;
            _downloadersService = downloadersService;
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
            await _downloadersService.ValidateDownloader(downloader);
            _context.Downloaders.Add(downloader);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDownloadById),
                new { id = downloader.DownloaderId },
                downloader);
        }

        [HttpPost("{id}/downloads/file")]
        public async Task<ActionResult<Download>> AddDownloderDownload([FromForm][Required] IFormFile file, int id)
        {
            var downloader = await _downloadersService.GetById(id);
            if (downloader == null) {
                return NotFound($"Downloader with id {id} not found");
            }
            List<Download> downloads;
            using (var stream = file.OpenReadStream()) {
                downloads = await _downloadersService.getDownloaderEngine(downloader).AddDownloadFromFile(downloader, stream, name: file.FileName, size: file.Length).ToListAsync();
            }
            _logger.LogInformation($"Received download file: {file.FileName} ({file.Length})");
            return Ok(downloads);
        }
    }
}
