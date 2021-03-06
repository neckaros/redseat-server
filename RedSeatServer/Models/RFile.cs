using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace RedSeatServer.Models
{
    
    class RFileMapperProfile : Profile {
        public RFileMapperProfile()
        {
            CreateMap<RFile, RFileDto>();
        }
    }
    public class RFile {
        [Key]
        public int fileId {get;set;}
        public Download Download {get;set;}
        public string Path { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public Show Show {get;set;}
        public Episode Episode {get;set;}
        public bool Parsed  {get;set;}
        
        public override string ToString() {
            if (Show != null) {
                return $"{Show.Name} {Episode.Season}x{Episode.Number}";
            } else return Name;
        }
    }

      public class RFileDto {
        [Key]
        public int fileId {get;set;}
        public string Name { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public ShowLightDto Show {get;set;}
        public Episode Episode {get;set;}
        public bool Parsed  {get;set;}
    }
    
}