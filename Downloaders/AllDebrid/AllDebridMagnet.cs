using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace redseat_server.Downloaders.AllDebrid
{
    public class AllDebridMagnet
    {
        public AllDebridMagnet() {}
        [JsonPropertyName("id")]
        public int Id { get; set; } 

        [JsonPropertyName("filename")]
        public string Filename { get; set; } 

        [JsonPropertyName("size")]
        public long Size { get; set; } 

        [JsonPropertyName("status")]
        public string Status { get; set; } 

        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } 

        [JsonPropertyName("downloaded")]
        public long Downloaded { get; set; } 

        [JsonPropertyName("uploaded")]
        public long Uploaded { get; set; } 

        [JsonPropertyName("seeders")]
        public int Seeders { get; set; } 

        [JsonPropertyName("downloadSpeed")]
        public int DownloadSpeed { get; set; } 

        [JsonPropertyName("uploadSpeed")]
        public int UploadSpeed { get; set; } 

        [JsonPropertyName("uploadDate")]
        public int UploadDate { get; set; } 

        [JsonPropertyName("links")]
        public List<AllDebridLink> Links { get; set; } 

    
    }

}