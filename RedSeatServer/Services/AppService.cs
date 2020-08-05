using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Collections.Generic;

namespace RedSeatServer.Services
{
    
    public class IpResult {
        public string ip {get;set;}
    }
    public interface IAppService
    {
        IAsyncEnumerable<IpResult> GetExternalIp();
    }

    public class AppService : IAppService
    {
        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    
        private IHttpClientFactory _clientFactory;
        private readonly IServer _server;

        public AppService(IHttpClientFactory clientFactory, IServer server)
        {
            
            _clientFactory = clientFactory;
            _server = server;
        }


        public async IAsyncEnumerable<IpResult> GetExternalIp()
        {
            var client = _clientFactory.CreateClient();
            var streamTask = client.GetStreamAsync("https://api.ipify.org?format=json");
            var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses?.ToArray();
            foreach (var address in addresses)
            {
                yield return new IpResult() {ip = address};
            }
            var result = await JsonSerializer.DeserializeAsync<IpResult>(await streamTask, jsonOptions);
            if (result != null) {
                result.ip = $"https://{result.ip}";
            }
            yield return result;
        }
    }

}