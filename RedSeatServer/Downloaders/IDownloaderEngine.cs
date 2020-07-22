using System.Collections.Generic;
using System.Threading.Tasks;
using RedSeatServer.Models;

namespace RedSeatServer.Downloaders
{
    public interface IDownloaderEngine
    {        
        static DownloaderType Name {get;}
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        IAsyncEnumerable<Download> AddDownloadFromFile(Downloader downloader, System.IO.Stream fileStream, string name = null, long? size = null );
        Task ValidateDownloader(Downloader downloader);
    }

}