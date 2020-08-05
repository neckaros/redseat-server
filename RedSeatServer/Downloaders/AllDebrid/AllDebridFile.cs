using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public class AllDebridFile
    {
        public AllDebridFile() { }
        [JsonPropertyName("file")]
        public string File { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("hash")]
        public string Hash { get; set; }

        [JsonPropertyName("ready")]
        public bool Ready { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

    }
}