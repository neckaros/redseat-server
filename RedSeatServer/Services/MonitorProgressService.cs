using System.Threading.Tasks;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;
using Hangfire;
using System.Linq;

namespace RedSeatServer.Services
{
    public class MonitorProgressService
    {
        private RedseatDbContext _dbContext;
        private readonly IShowService _showService;
        private readonly IDownloadersService _downloaderService;
        private readonly IDownloadsService _downloadsService;
        private readonly ILogger _logger;
        private readonly IParserService _parserService;
        private readonly IBackgroundJobClient _backgroundJobs;

        public MonitorProgressService(RedseatDbContext dbContext, IShowService showService, IDownloadersService downloaderService, IDownloadsService downloadsService, ILogger<MonitorProgressService> logger, IParserService parserService, IBackgroundJobClient backgroundJobs)
        {
            _dbContext = dbContext;
            _showService = showService;
            _downloaderService = downloaderService;
            _downloadsService = downloadsService;
            _logger = logger;
            _parserService = parserService;
            _backgroundJobs = backgroundJobs;
        }

        public async Task CheckFilesNeedingParsing() {
            var files = await _downloadsService.GetUnparsedFiles();
            foreach (var file in files.Where(f => f.Name.EndsWith("mkv") || f.Name.EndsWith("mp4")))
            {
                _backgroundJobs.Enqueue<MonitorProgressService>((m) => m.ParseFile(file.fileId, false));
            }
            
        }

        
        public async Task ParseFile(int fileId, bool force) {
            var file = await _downloadsService.GetFile(fileId);
            if (file.Parsed && !force) {
                return;
            }
            else if (file.Download.Type == DownloadType.Show) {
                Console.WriteLine(file.Name);
                var parsed = _parserService.ParseTitle(file.Name);
                if (parsed != null && parsed.SeriesTitle != null) {
                    _logger.LogInformation($"Parsed Show {parsed.SeriesTitle} {parsed.SeasonNumber}x{parsed.EpisodeNumbers.FirstOrDefault()}");
                    var id = await _showService.tvdbIdByTitle(parsed.SeriesTitle);
                    var show = await _showService.getShowByTvdbId(id);
                    file.Show = show;
                    file.Parsed = true;
                    file.Episode = show.Episodes.FirstOrDefault(e => e.Season == parsed.SeasonNumber && e.Number == parsed.EpisodeNumbers.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }
            Console.WriteLine(file.Download.Type);
            
        }

        public async Task CheckAllDownloads()
        {
            // Do some job here
            var downloaders = await _dbContext.Downloaders.AsQueryable().ToListAsync();
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