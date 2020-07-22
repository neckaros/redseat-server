using System.Threading.Tasks;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;

namespace RedSeatServer.Services
{
    public class MonitorProgressService
    {
        private RedseatDbContext _dbContext;
        private readonly IDownloadersService _downloaderService;
        private readonly ILogger _logger;
        private readonly IParserService _parserService;

        public MonitorProgressService(RedseatDbContext dbContext, IDownloadersService downloaderService, ILogger<MonitorProgressService> logger, IParserService parserService)
        {
            _dbContext = dbContext;
            _downloaderService = downloaderService;
            _logger = logger;
            _parserService = parserService;
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
                    
                    // foreach (var file in magnet.Files)
                    // {
                    //     var info = _parserService.ParseTitle(file);
                    //     if (info != null)
                    //         _logger.LogInformation($"{info.SeriesTitle} - S{info.SeasonNumber}x{String.Join(',', info.EpisodeNumbers)} ({info.Quality?.Quality})");
                    // }
                }
            }
        }
    }

}