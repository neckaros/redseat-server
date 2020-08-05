using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace RedSeatServer.Models
{
    public class DownloadProgress
    {
        public Downloader Downloader {get; set;}
        public string DownloaderId {get; set; }
        
        public string Name {get; set; }
        public IEnumerable<RFile> Files {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        
        public long Size { get; set; }
        public long Downloaded { get; set; }
        public double Progress { get => Downloaded / Size; }
    }
}