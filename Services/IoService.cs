using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using redseat_server.Downloaders.AllDebrid;
using redseat_server.Models;

namespace redseat_server.Services
{
    public class IoService : IIoService
    {
        public string DefaultPath => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public IEnumerable<string> GetFolders(string path)
        {
            if (path == null) path = DefaultPath;
            return Directory.EnumerateDirectories(path);
        }
    }
}