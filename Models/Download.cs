using System.Text.Json.Serialization;


namespace redseat_server.Models
{
    public class Download
    {
        public int DownloadId { get; set; }
        public Downloader Downloader {get; set;}
        public string DownloaderId {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public double Progress { get; set; }
    }
}