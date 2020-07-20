using System.Collections.Generic;
using System.Threading.Tasks;
using redseat_server.Models;

namespace redseat_server.Downloaders
{
    public interface IDownloaderService
    {
        IAsyncEnumerable<DownloadProgress> GetAllDownloads(Downloader downloader);
        Task ValidateDownloader(Downloader downloader);
    }

}