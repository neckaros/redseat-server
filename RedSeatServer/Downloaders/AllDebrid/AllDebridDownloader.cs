using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RedSeatServer.Downloaders.AllDebrid;
using RedSeatServer.Models;

namespace RedSeatServer.Downloaders
{
    public class AllDebridDownloader : IDownloaderEngine
    {
        private IHttpClientFactory _clientFactory;
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        public AllDebridDownloader(IHttpClientFactory clientFactory) {
            _clientFactory = clientFactory;

        }

        public async IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader)
        {
            var client = _clientFactory.CreateClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridResultMagnets>>(await streamTask, jsonOptions);
            foreach (var magnet in result.data.Magnets)
            {
                yield return new DownloadProgress(){Files = magnet.Links.Select((l) => l.Filename).ToList(), Progress = magnet.Downloaded / magnet.Size, Downloader = downloader, DownloaderId = magnet.Id.ToString(), Name = magnet.Filename };
            }
        }

        public async Task ValidateDownloader(Downloader downloader)
        {
            if (downloader.Path != null) {
                throw new DownloaderVerificationException("No path must be set for AllDebrid downloader");
            }
            if (downloader.Token == null) {
                throw new DownloaderVerificationException("AllDebrid downloader require the use of a token");
            }
            
            var client = _clientFactory.CreateClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"https://api.alldebrid.com/v4/user?agent=myAppName&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridUser>>(await streamTask, jsonOptions);
            
            if (result.status != AllDebridStatus.success) {
                throw new DownloaderVerificationException("AllDebrid token seem invalid");
            }
            
        }

        public async IAsyncEnumerable<Download> AddDownloadFromFile(Downloader downloader, Stream fileStream, string name = null, long? size = null)
        {
            yield return new Download(){ Downloader = downloader, ExternalId = "1", size = 10, DownloadStatus = DownloadStatus.Pending};
        }
    }
}