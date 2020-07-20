using System.Threading.Tasks;
using redseat_server.Models;
using Microsoft.EntityFrameworkCore;
using System;
using redseat_server.Downloaders;
using Microsoft.Extensions.Logging;

namespace redseat_server.Services
{
    public class MonitorProgressService
    {
        private RedseatDbContext _dbContext;
        private readonly IDownloaderService _downloaderService;
        private readonly ILogger _logger;

        public MonitorProgressService(RedseatDbContext dbContext, IDownloaderService downloaderService, ILogger<MonitorProgressService> logger )
        {
            _dbContext = dbContext;
            _downloaderService = downloaderService;
            _logger = logger;
        }


        public async Task CheckAllDownloads()
        {
            // Do some job here
            var downloaders = await _dbContext.Downloaders.ToListAsync();
            _logger.LogInformation($"CheckAllDownloads got {downloaders.Count} downloader");
            foreach (var downloader in downloaders)
            {
                _logger.LogInformation($"Getting all downloads for downloader {downloader.DownloaderId} ({downloader.DownloaderType.ToString()})");
                
                var downloadsProgress = _downloaderService.GetAllDownloads(downloader);
                await foreach (var magnet in downloadsProgress)
                {
                    var progress = Math.Round(magnet.Progress * 100);
                    _logger.LogInformation($"Got download {magnet.Name} - {progress}% (Downloader {downloader.DownloaderId} ({downloader.DownloaderType.ToString()}))");
                }
            }
        }
    }

}