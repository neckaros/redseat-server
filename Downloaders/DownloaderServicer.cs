using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using redseat_server.Downloaders.AllDebrid;
using redseat_server.Models;

namespace redseat_server.Downloaders
{
    public class DownloaderService : IDownloaderService
    {
        private readonly IServiceProvider _serviceProvider;

        public DownloaderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IDownloader getDownloaderService(Downloader downloader) {
            if (downloader.DownloaderType == DownloaderType.Alldebrid) {
                return ActivatorUtilities.CreateInstance<AllDebridDownloader>(_serviceProvider);
            } else {
                throw new NotSupportedException($"{downloader.DownloaderType.ToString()} is not a recognized downlaoder");
            }
        }
        public IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader) => getDownloaderService(downloader).GetAllDownloads(downloader);

        public Task ValidateDownloader(Downloader downloader) {
            if (downloader.Name == null) {
                throw new DownloaderVerificationException("Name must not be null");
            }
            return getDownloaderService(downloader).ValidateDownloader(downloader);
        }
    }
}