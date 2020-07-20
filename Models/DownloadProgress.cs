using System.Text.Json.Serialization;


namespace redseat_server.Models
{
    public class DownloadProgress
    {
        public Downloader Downloader {get; set;}
        public string DownloaderId {get; set; }
        
        public string Name {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public double Progress { get; set; }
    }
}