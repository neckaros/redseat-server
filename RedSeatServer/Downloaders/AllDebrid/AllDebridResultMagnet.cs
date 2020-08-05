using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public class AllDebridResultMagnet   {
        public AllDebridResultMagnet() {}
        [JsonPropertyName("magnets")]
        public AllDebridMagnet Magnets { get; set; } 

    }
}