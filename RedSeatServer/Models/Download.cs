using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace RedSeatServer.Models
{
    class DownloadMapperProfile : Profile {
        public DownloadMapperProfile()
        {
            CreateMap<Download, DownloadDto>();
            CreateMap<Download, DownloadDtoWithoutDownloader>();
        }
    }
    

    public enum DownloadType
    {
        None,
        Show,
        Movie,
        Book,
        Manga,
        Anime
    }

    public class Download
    {
        [Key]
        public int DownloadId { get; set; }
        public Downloader Downloader {get; set; }
        public string ExternalId {get; set; }

        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public long Size { get; set; }
        public long Downloaded { get; set; }
        public List<RFile> Files { get; set; }
    }

    public class DownloadDto {
        public int DownloadId { get; set; }
        public DownloaderDtoWithoutDownloads Downloader {get; set; }
        public string ExternalId {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public long Size { get; set; }
        public long Downloaded { get; set; }
        
        public List<RFileDto> Files { get; set; }
    }
     public class DownloadDtoWithoutDownloader {
        public int DownloadId { get; set; }
        public string ExternalId {get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DownloadStatus DownloadStatus { get; set; }
        public long Size { get; set; }
        public long Downloaded { get; set; }
        
        public List<RFileDto> Files{ get; set; }
    }
}