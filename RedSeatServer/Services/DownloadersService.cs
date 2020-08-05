using System.Threading.Tasks;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace RedSeatServer.Services
{
    public class RsFileStream {
        public System.IO.Stream Stream {get;set;}
        public long Length {get;set;}
    }
    public interface IDownloadersService
    {
        
        Task<RsFileStream> GetFileStream(int fileId);
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        ValueTask<Downloader> GetById(int id);
        ValueTask<Download> AddDownload(Download download);
        IDownloaderEngine getDownloaderEngine(Downloader downloader);
        Task ValidateDownloader(Downloader downloader);
    }

    public class DownloadersService : IDownloadersService
    {
        private RedseatDbContext _dbContext;
        private readonly ILogger _logger;

        private readonly IServiceProvider _serviceProvider;

        public DownloadersService(RedseatDbContext dbContext, ILogger<MonitorProgressService> logger, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IDownloaderEngine getDownloaderEngine(Downloader downloader)
        {
            if (downloader.DownloaderType == DownloaderType.Alldebrid)
            {
                return ActivatorUtilities.CreateInstance<AllDebridDownloader>(_serviceProvider);
            }
            else
            {
                throw new NotSupportedException($"{downloader.DownloaderType.ToString()} is not a recognized downlaoder");
            }
        }
        public IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader) => getDownloaderEngine(downloader).GetAllDownloads(downloader);

        public Task ValidateDownloader(Downloader downloader)
        {
            if (downloader.Name == null)
            {
                throw new DownloaderVerificationException("Name must not be null");
            }
            return getDownloaderEngine(downloader).ValidateDownloader(downloader);
        }

        public ValueTask<Downloader> GetById(int id)
        {
            return _dbContext.Downloaders.FindAsync(id);
        }

        public async Task<RsFileStream> GetFileStream(int fileId) {
            var file = await _dbContext.Files.Include(f => f.Download)
    .ThenInclude(d => d.Downloader).Where(f => f.fileId == fileId).FirstOrDefaultAsync();
            return await getDownloaderEngine(file.Download.Downloader).GetFileStream(file.Download.Downloader, file);
        }

        public async ValueTask<Download> AddDownload(Download download) {
            var downloadSaved = await _dbContext.Downloads.AddAsync(download);
            await _dbContext.SaveChangesAsync();
            return downloadSaved.Entity;
        }

        
        public async ValueTask<object> ParseDownload(int downloadId) {
            var download = await _dbContext.Downloads.FindAsync(downloadId);
            
            return download;
        }
    }

}