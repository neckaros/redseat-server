using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RedSeatServer.Downloaders.AllDebrid;
using RedSeatServer.Models;
using RedSeatServer.Services;

namespace RedSeatServer.Downloaders
{
    public class AllDebridDownloader : IDownloaderEngine
    {
        private IHttpClientFactory _clientFactory;
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        public AllDebridDownloader(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;

        }

        public HttpClient GetClient()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.alldebrid.com/v4/");
            return client;
        }

        public async IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader)
        {
            var client = GetClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"magnet/status?agent=myAppName&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridResultMagnets>>(await streamTask, jsonOptions);
            foreach (var magnet in result.data.Magnets)
            {
                yield return new DownloadProgress() { Files = magnet.Links.Select((l) => new RFile(){Name = l.Filename, Size = l.Size}).ToList(), Downloaded = magnet.Downloaded, Size = magnet.Size, Downloader = downloader, DownloaderId = magnet.Id.ToString(), Name = magnet.Filename };
            }
        }

        

        public async ValueTask<DownloadProgress> GetDownload(Downloader downloader, object id)
        {
            if (!int.TryParse(id.ToString(), out var idParsed)) throw new ArgumentException("Id must be a int or parsable int for AllDebrid");
            var client = GetClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"magnet/status?id={idParsed}&agent=myAppName&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridResultMagnet>>(await streamTask, jsonOptions);
            var magnet = result.data.Magnets;
            return new DownloadProgress() { Files = magnet.Links.Select((l) => new RFile(){Name = l.Filename, Size = l.Size, Path = l.Link}).ToList(), Downloaded = magnet.Downloaded, Size = magnet.Size, Downloader = downloader, DownloaderId = magnet.Id.ToString(), Name = magnet.Filename };
            
        }

        public async Task ValidateDownloader(Downloader downloader)
        {
            if (downloader.Path != null)
            {
                throw new DownloaderVerificationException("No path must be set for AllDebrid downloader");
            }
            if (downloader.Token == null)
            {
                throw new DownloaderVerificationException("AllDebrid downloader require the use of a token");
            }

            var client = GetClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"user?agent=myAppName&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridUser>>(await streamTask, jsonOptions);

            if (result.status != AllDebridStatus.success)
            {
                throw new DownloaderVerificationException("AllDebrid token seem invalid");
            }

        }

        public async IAsyncEnumerable<Download> AddDownloadFromFile(Downloader downloader, Stream fileStream, string name = null, long? size = null, DownloadType type = DownloadType.None)
        {
            var client = GetClient();
            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StreamContent(fileStream), "files[0]", name ?? "torrent.torrent");
            var request = await client.PostAsync($"magnet/upload/file?agent=myAppName&apikey={downloader.Token}", multipartContent);
            if( request.StatusCode == HttpStatusCode.OK) {
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridResultFiles>>(await request.Content.ReadAsStreamAsync(), jsonOptions);
            
            if (result.status == AllDebridStatus.success) {
            foreach (var file in result.data.Files)
            {
                DownloadProgress progress = null;
                if (file.Ready) {
                    progress = await GetDownload(downloader, file.Id);
                }
                yield return new Download()
                {
                    Type = type,
                    Downloader = downloader,
                    ExternalId = file.Id.ToString(),
                    Size = progress?.Size ?? file.Size,
                    DownloadStatus = file.Ready ? DownloadStatus.Downloaded : DownloadStatus.Downloading,
                    Downloaded = progress?.Downloaded ?? 0,
                    Files = progress?.Files.ToList(),
                    FilesAvailable = progress?.Files != null,
                };
            }
            } else {
                throw new InvalidOperationException();
            }
            } else {
                Console.WriteLine(request.StatusCode);
            }
        }

        public async Task<RsFileStream> GetFileStream(Downloader downloader, RFile file)
        {
            var client = GetClient();
            //https://api.alldebrid.com/v4/magnet/status?agent=myAppName&apikey=someValidApikeyYouGenerate
            var streamTask = client.GetStreamAsync($"link/unlock?agent=myAppName&link={file.Path}&apikey={downloader.Token}");
            var result = await JsonSerializer.DeserializeAsync<AllDebridResult<AllDebridLink>>(await streamTask, jsonOptions);
            var lien = result.data.Link;
            
            var clientStream = _clientFactory.CreateClient();
            var getTask = client.GetAsync(lien, HttpCompletionOption.ResponseHeadersRead);
            HttpResponseMessage response = await getTask.ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var lengthHeader = response.Content.Headers.TryGetValues("Content-Length", out var lengthStringArray);
            var lengthString = lengthStringArray?.FirstOrDefault();
            long.TryParse(lengthString, out var length);
            HttpContent c = response.Content;
            var stream = c != null ? await c.ReadAsStreamAsync().ConfigureAwait(false) :
                System.IO.Stream.Null;
            return new RsFileStream() {Stream = stream, Length = length};
        }
    }
}