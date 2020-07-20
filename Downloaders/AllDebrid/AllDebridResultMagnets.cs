using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace redseat_server.Downloaders.AllDebrid
{
    public class AllDebridResultMagnets   {
        public AllDebridResultMagnets() {}
        [JsonPropertyName("magnets")]
        public List<AllDebridMagnet> Magnets { get; set; } 

    }
}