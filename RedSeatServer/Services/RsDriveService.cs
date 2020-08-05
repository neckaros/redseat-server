using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RedSeatServer.Extensions;
using System.Net.Http.Headers;
using RedSeatServer.Models;
using System.Web;

namespace RedSeatServer.Services
{

    public class ServiceToken
    {
        public string accessToken { get; set; }
        public DateTimeOffset expiresAt { get; set; }
        public string provider { get; set; }
        public string type { get; set; }
    }
    public class TokenResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    public class DriveFileResult {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } 

        [JsonPropertyName("nextPageToken")]
        public string NextPageToken { get; set; } 

        [JsonPropertyName("incompleteSearch")]
        public bool IncompleteSearch { get; set; } 
        public DriveFile[] Files {get;set;}
    }

    class MessageTokenHandler : DelegatingHandler
    {
        
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        private readonly string _tokenId;
        private readonly string _tokenKey;

        private ServiceToken _token;

        public IRsDriveService driveService {get; set;}
        public MessageTokenHandler(string tokenId, string tokenKey)
        {
            InnerHandler = new HttpClientHandler();
            _tokenId = tokenId;
            _tokenKey = tokenKey;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var token = await GetToken();
            request.Headers.Add("Authorization", "Bearer " + token.accessToken);
            return await base.SendAsync(request, cancellationToken);
        }

        
        public async Task<ServiceToken> GetToken() {
            await maybeRefreshToken();
            return _token;
        }
        public async Task maybeRefreshToken()
        {
            if (_token?.accessToken == null || _token.expiresAt == null || _token.expiresAt < DateTimeOffset.Now.AddMinutes(-2))
            {
                await refreshToken();
            }
        }
        public async Task refreshToken()
        {

            using(var client = new HttpClient()) {
            client.DefaultRequestHeaders.Authorization 
                         = new AuthenticationHeaderValue("Bearer", _tokenKey);
            var request = await client.GetAsync($"http://localhost:3000/api/token?id={_tokenId}");
            var result = await JsonSerializer.DeserializeAsync<TokenResult>(await request.Content.ReadAsStreamAsync(), jsonOptions);
            _token = new ServiceToken() { 
            accessToken = result.AccessToken,
            expiresAt = DateTimeOffset.Now.AddSeconds(result.ExpiresIn)};
            }
        }
    }

    public class DriveFile {
        public string Id {get;set;}
        public string Name {get;set;}
        public string MimeType {get;set;}
        public string[] parents {get; set;}
    }
    public interface IRsDriveService
    {
        Task<IEnumerable<DriveFile>> Browse(string parent, string tokenId, string tokenKey);
        Task upload(int fileId, string tokenId, string tokenKey, string name, string parent);
        Task upload(System.IO.Stream stream, string tokenId, string tokenKey, string name, string parent, long? length = null);
    }



    public class RsDriveService : IRsDriveService
    {
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private string _apiKey;
        static string[] Scopes = { DriveService.Scope.Drive };
        static string ApplicationName = "RedSeat";
        private readonly ILogger<RsDriveService> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDownloadersService _downloadersService;

        public RsDriveService(ILogger<RsDriveService> logger, IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor, IDownloadersService downloadersService)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _httpContextAccessor = httpContextAccessor;
            _downloadersService = downloadersService;
        }

        private void Request_ProgressChanged(Google.Apis.Upload.IUploadProgress obj)
        {
            _logger.LogInformation(obj.Status + " " + obj.BytesSent);
        }

        private void Request_ResponseReceived(Google.Apis.Drive.v3.Data.File obj)
        {
            if (obj != null)
            {
                _logger.LogWarning("File was uploaded sucessfully--" + obj.Id);
            }
        }

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        public async Task upload(int file, string tokenId, string tokenKey, string name, string parent)
        {
            
            // var client = _clientFactory.CreateClient();
            // var getTask = client.GetAsync(link, HttpCompletionOption.ResponseHeadersRead);
            // HttpResponseMessage response = await getTask.ConfigureAwait(false);
            // response.EnsureSuccessStatusCode();
            // var lengthHeader = response.Content.Headers.TryGetValues("Content-Length", out var lengthStringArray);
            // var lengthString = lengthStringArray?.FirstOrDefault();
            // long.TryParse(lengthString, out var length);
            // HttpContent c = response.Content;
            // var stream = c != null ? await c.ReadAsStreamAsync().ConfigureAwait(false) :
            //     System.IO.Stream.Null;
            var streamResult = await _downloadersService.GetFileStream(file);
                await upload(streamResult.Stream, tokenId, tokenKey, name, parent, streamResult.Length);
        }

        public async Task upload(System.IO.Stream streamFile, string tokenId, string tokenKey, string name, string parent, long? length = null)
        {
            var tokenHandler = new MessageTokenHandler(tokenId, tokenKey) {driveService = this};
            var client = new HttpClient(tokenHandler);
            var driveFile = new DriveFile() { Name = name, MimeType = "video/x-matroska", parents = new string[]{parent} };
            var response = await client.PostAsJsonAsync("https://www.googleapis.com/upload/drive/v3/files?uploadType=resumable", driveFile, jsonOptions);
            var location = response.Headers.GetValues("Location").FirstOrDefault();
            var resumableUpload = ResumableUpload.CreateFromUploadUri(new Uri(location), streamFile);
            
            resumableUpload.ProgressChanged += (Google.Apis.Upload.IUploadProgress obj) => { 
                if (length != null && length.Value > 0) {
                double progressPercent = (double)obj.BytesSent / (double)length.Value * 100;
                
                _logger.LogInformation($"Uploaded file {Math.Round(progressPercent, 2)}% {obj.BytesSent.HumanReadableFileSize()} / {length.Value.HumanReadableFileSize()}");
                } else {
                    _logger.LogInformation($"Uploaded {obj.BytesSent.HumanReadableFileSize()} bytes");
                }
            };

            await resumableUpload.UploadAsync();
            

        }

        public async Task<IEnumerable<DriveFile>> Browse(string parent, string tokenId, string tokenKey)
        {
            var builder = new UriBuilder("https://www.googleapis.com/drive/v3/files");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["q"] = $"mimeType = 'application/vnd.google-apps.folder' and '{parent ?? "root"}' in parents";
            query["pageSize"] = "1000";
            builder.Query = query.ToString();
            var tokenHandler = new MessageTokenHandler(tokenId, tokenKey) {driveService = this};
            var client = new HttpClient(tokenHandler);
            var response = await client.GetFromJsonAsync<DriveFileResult>(builder.Uri, jsonOptions);
            return response.Files;
        }


        public async Task<IEnumerable<DriveFile>> GetFiles(string parent, string tokenId, string tokenKey, string nextPageToken = null)
        {
            var builder = new UriBuilder("https://www.googleapis.com/drive/v3/files");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["q"] = $"mimeType = 'application/vnd.google-apps.folder' and '{parent ?? "root"}' in parents";
            query["pageSize"] = "1000";
            query["pageToken"] = nextPageToken;
            builder.Query = query.ToString();
            var tokenHandler = new MessageTokenHandler(tokenId, tokenKey) {driveService = this};
            var client = new HttpClient(tokenHandler);
            var response = await client.GetFromJsonAsync<DriveFileResult>(builder.Uri, jsonOptions);
            var files = response.Files;
            if (response.NextPageToken != null) {
                var otherFiles = await GetFiles(parent, tokenId, tokenKey, response.NextPageToken);
                files = files.Union(otherFiles).ToArray();
            }
            return files;
        }
    }
}