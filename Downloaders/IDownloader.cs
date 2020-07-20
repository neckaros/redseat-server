using System.Collections.Generic;
using System.Threading.Tasks;
using redseat_server.Models;

namespace redseat_server.Downloaders
{
    public interface IDownloader
    {        
        static DownloaderType Name {get;}
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        Task ValidateDownloader(Downloader downloader);
    }

}