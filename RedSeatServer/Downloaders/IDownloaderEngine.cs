using System.Collections.Generic;
using System.Threading.Tasks;
using RedSeatServer.Models;
using RedSeatServer.Services;

namespace RedSeatServer.Downloaders
{
    public interface IDownloaderEngine
    {        
        static DownloaderType Name {get;}
        
        Task<RsFileStream> GetFileStream(Downloader downloader, RFile file);
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        ValueTask<DownloadProgress> GetDownload(Downloader downloader, object id);
        IAsyncEnumerable<Download> AddDownloadFromFile(Downloader downloader, System.IO.Stream fileStream, string name = null, long? size = null, DownloadType type = DownloadType.None );
        Task ValidateDownloader(Downloader downloader);
    }

}