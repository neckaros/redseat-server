using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace RedSeatServer.Models
{
    public class Download
    {
        [Key]
        public int DownloadId { get; set; }
        public Downloader Downloader {get; set; }
        public string ExternalId {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public long size { get; set; }
        public long downloaded { get; set; }
    }
}