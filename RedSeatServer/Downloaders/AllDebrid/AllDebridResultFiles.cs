using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public class AllDebridResultFiles   {
        public AllDebridResultFiles() {}
        [JsonPropertyName("files")]
        public List<AllDebridFile> Files { get; set; } 

    }
}