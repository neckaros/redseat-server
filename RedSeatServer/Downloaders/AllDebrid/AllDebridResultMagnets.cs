using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public class AllDebridResultMagnets   {
        public AllDebridResultMagnets() {}
        [JsonPropertyName("magnets")]
        public List<AllDebridMagnet> Magnets { get; set; } 

    }
}