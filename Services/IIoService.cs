using System.Collections.Generic;

namespace redseat_server.Services
{
    public interface IIoService
    {
        string DefaultPath {get;}

        
        IEnumerable<string> GetFolders(string path);
    }
}