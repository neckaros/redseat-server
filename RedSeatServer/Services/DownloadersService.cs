using System.Threading.Tasks;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace RedSeatServer.Services
{
    public interface IDownloadersService
    {
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        ValueTask<Downloader> GetById(int id);
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
    }

}