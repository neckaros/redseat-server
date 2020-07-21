using System.Collections.Generic;

namespace RedSeatServer.Services
{
    public interface IIoService
    {
        string DefaultPath {get;}

        
        IEnumerable<string> GetFolders(string path);
    }
}