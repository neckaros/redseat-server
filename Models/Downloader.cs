using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;


namespace redseat_server.Models
{
    public enum DownloaderType {
        None,
        Alldebrid,
    }
    public class Downloader
    {
        public int DownloaderId { get; set; }
        public List<Download> Downloads {get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public DownloaderType DownloaderType { get; set; }
        public String Name { get; set; }
        public string Path {get; set;}
        public string Token {get; set;}

        public override string ToString() {
            return $"{Name} (id: {DownloaderId} - type: {DownloaderType})";
        }
    }
}