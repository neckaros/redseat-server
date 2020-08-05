using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;
using AutoMapper;
using RedSeatServer.Extensions;

namespace RedSeatServer.Models
{
    public enum ShowStatus {
        Unknown,
        Upcoming,
        Continuing,
        Ended,
    }
    class ShowMapperProfile : Profile {
        public ShowMapperProfile()
        {
            CreateMap<TvDbSharper.Dto.Series, Show>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.SeriesName))
            .ForMember(dest => dest.TvdbId, opt => opt.MapFrom(src => src.Id))
        
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Select((g) => new Genre(){Name = g}).ToList()))
            .ForMember(dest => dest.Runtime, opt => opt.MapFrom(src => src.Runtime == null ? (int?)null : int.Parse(src.Runtime)))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.LastUpdated)))
            .ForMember(dest => dest.AirsDayOfWeek, opt => opt.MapFrom(src => src.AirsDayOfWeek.ToDayOfWeek()))
            .ForMember(dest => dest.AirsTime, opt => opt.MapFrom(src => src.AirsTime.TimeOfDay()))
            .ForMember(dest => dest.Added, opt => opt.MapFrom(src => DateTimeOffset.ParseExact(src.Added, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))

            .ReverseMap()
            
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Select((g) => g.Name).ToArray()))
            .ForMember(dest => dest.Runtime, opt => opt.MapFrom(src => src.Runtime.ToString()))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated.ToUnixTimeSeconds()))
            .ForMember(dest => dest.AirsDayOfWeek, opt => opt.MapFrom(src => src.AirsDayOfWeek.ToString()))
            .ForMember(dest => dest.AirsTime, opt => opt.MapFrom(src => src.AirsTime.TimeOfDayString()))
            .ForMember(dest => dest.Added, opt => opt.MapFrom(src => src.Added.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))
            
;
            
        }
    }
    public class Show
    {
        public int ShowId {get; set;}
        [JsonPropertyName("id")]
        public int TvdbId { get; set; } 
        [JsonPropertyName("name")]
        public string Name { get; set; } 

        [JsonPropertyName("aliases")]
        public string[] Aliases { get; set; } 

        [JsonPropertyName("season")]
        public string Season { get; set; } 
        
        [JsonPropertyName("status")]
        public ShowStatus Status { get; set; } 

        [JsonPropertyName("firstAired")]
        public DateTimeOffset? FirstAired { get; set; } 

        [JsonPropertyName("runtime")]
        public int? Runtime { get; set; } 

        [JsonPropertyName("language")]
        public string Language { get; set; } 

        [JsonPropertyName("genre")]
        public List<Genre> Genre { get; set; } 

        [JsonPropertyName("overview")]
        public string Overview { get; set; } 

        [JsonPropertyName("lastUpdated")]
        public DateTimeOffset LastUpdated { get; set; } 

        [JsonPropertyName("airsDayOfWeek")]
        public DayOfWeek? AirsDayOfWeek { get; set; } 

        [JsonPropertyName("airsTime")]
        public TimeSpan AirsTime { get; set; } 

        [JsonPropertyName("rating")]
        public string Rating { get; set; } 

        [JsonPropertyName("imdbId")]
        public string ImdbId { get; set; } 

        [JsonPropertyName("zap2itId")]
        public string Zap2itId { get; set; } 

        [JsonPropertyName("added")]
        public DateTimeOffset Added { get; set; } 

        [JsonPropertyName("slug")]
        public string Slug { get; set; } 

        
        [JsonPropertyName("episodes")]
        public List<Episode> Episodes { get; set; } 

    }
}