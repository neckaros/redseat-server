using System.Collections.Generic;
using System.Threading.Tasks;
using RedSeatServer.Models;

namespace RedSeatServer.Downloaders
{
    public interface IDownloader
    {        
        static DownloaderType Name {get;}
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        Task ValidateDownloader(Downloader downloader);
    }

}