using System.Text.Json.Serialization;

namespace RedSeatServer.Downloaders.AllDebrid
{
    public enum AllDebridStatus { success, error }
    public class AllDebridResult<T>
    {
        public AllDebridResult() {}
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AllDebridStatus status {get; set;}
        public T data {get; set;}
    }

}