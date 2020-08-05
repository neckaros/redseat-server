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
using AutoMapper;
using TvDbSharper;
using Hangfire;

namespace RedSeatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloaderController : ControllerBase
    {
        private readonly ILogger<DownloaderController> _logger;
        private readonly RedseatDbContext _context;
        private readonly IParserService _parserService;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IDownloadersService _downloadersService;
        private readonly IMapper _mapper;

        public DownloaderController(ILogger<DownloaderController> logger, RedseatDbContext context, IParserService parserService,  IBackgroundJobClient backgroundJobs, IDownloadersService downloadersService, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _parserService = parserService;
            _backgroundJobs = backgroundJobs;
            _downloadersService = downloadersService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Downloader>>>  Get()
        {
            return await _context.Downloaders.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DownloaderDto>> GetDownloadById(int id)
        {
            var downloader = await _context.Downloaders.FindAsync(id);

            if (downloader == null)
            {
                return NotFound();
            }

            return _mapper.Map<DownloaderDto>(downloader);
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
        public async Task<IEnumerable<DownloadDtoWithoutDownloader>> AddDownloderDownload([FromForm][Required] IFormFile file, int id, [FromForm] DownloadType type)
        {
            var downloader = await _downloadersService.GetById(id);
            if (downloader == null) {
                //return NotFound($"Downloader with id {id} not found");
            }
            List<Download> downloads = new List<Download>();
            using (var stream = file.OpenReadStream()) {
                var downloadsFromEngine = _downloadersService
                .getDownloaderEngine(downloader)
                .AddDownloadFromFile(downloader, stream, name: file.FileName, size: file.Length, type);
                await foreach (var downlaod in downloadsFromEngine)
                {
                    var downloadSaved = await _downloadersService.AddDownload(downlaod);
                    downloads.Add(downloadSaved);
                    foreach (var fileAdded in downloadSaved.Files)
                    {
                        _backgroundJobs.Enqueue<MonitorProgressService>((s) => s.ParseFile(fileAdded.fileId, true));
                    }
                    
                }
                
            await _context.SaveChangesAsync();
            }
            
            _logger.LogInformation($"Received download file: {file.FileName} ({file.Length})");
            var downloadsDto = _mapper.Map<List<DownloadDtoWithoutDownloader>>(downloads);
            return downloadsDto;
        }
    }
}
