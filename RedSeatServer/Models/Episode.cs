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
    class EpisodeMapperProfile : Profile {
        public EpisodeMapperProfile()
        {
            CreateMap<TvDbSharper.Dto.EpisodeRecord, Episode>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.EpisodeName))

            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.AiredEpisodeNumber))
            .ForMember(dest => dest.Season, opt => opt.MapFrom(src => src.AiredSeason))
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.LastUpdated)))
            .ForMember(dest => dest.FirstAired, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.FirstAired) ? DateTimeOffset.MinValue : DateTimeOffset.ParseExact(src.FirstAired, "yyyy-MM-dd", CultureInfo.InvariantCulture)))

            // .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Select((g) => new Genre(){Name = g}).ToList()))
            // .ForMember(dest => dest.Runtime, opt => opt.MapFrom(src => src.Runtime == null ? (int?)null : int.Parse(src.Runtime)))
            // .ForMember(dest => dest.AirsDayOfWeek, opt => opt.MapFrom(src => src.AirsDayOfWeek.ToDayOfWeek()))
            // .ForMember(dest => dest.AirsTime, opt => opt.MapFrom(src => src.AirsTime.TimeOfDay()))
            // .ForMember(dest => dest.Added, opt => opt.MapFrom(src => DateTimeOffset.ParseExact(src.Added, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))

            .ReverseMap()
            
            .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated.ToUnixTimeSeconds()))
            .ForMember(dest => dest.FirstAired, opt => opt.MapFrom(src => src.FirstAired.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)))

            // .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre.Select((g) => g.Name).ToArray()))
            // .ForMember(dest => dest.Runtime, opt => opt.MapFrom(src => src.Runtime.ToString()))
            // .ForMember(dest => dest.AirsDayOfWeek, opt => opt.MapFrom(src => src.AirsDayOfWeek.ToString()))
            // .ForMember(dest => dest.AirsTime, opt => opt.MapFrom(src => src.AirsTime.TimeOfDayString()))
            // .ForMember(dest => dest.Added, opt => opt.MapFrom(src => src.Added.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)))
            
;
            
        }
    }
    public class Episode
    {
        [JsonPropertyName("absoluteNumber")]
        public int AbsoluteNumber {get; set;}
        [JsonPropertyName("number")]
        public int Number { get; set; } 
        [JsonPropertyName("season")]
        public int Season { get; set; } 

        [JsonPropertyName("aired")]
        public DateTimeOffset FirstAired { get; set; } 

        [JsonPropertyName("LastUpdated")]
        public DateTimeOffset LastUpdated { get; set; } 

        [JsonPropertyName("title")]
        public string Title { get; set; } 

        [JsonPropertyName("overview")]
        public string Overview { get; set; } 


    }
}