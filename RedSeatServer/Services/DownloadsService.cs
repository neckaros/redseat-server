using System.Threading.Tasks;
using RedSeatServer.Models;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace RedSeatServer.Services
{
    public interface IDownloadsService
    {
        
        Task<List<Download>> GetAllDownloads();
        Task<List<Download>> GetUnfiledFownloads();
        Task<List<RFile>> GetUnparsedFiles();
        Task<RFile> GetFile(int id);
      
    }

    public class DownloadsService : IDownloadsService
    {
        private RedseatDbContext _dbContext;
        private readonly ILogger _logger;

        private readonly IServiceProvider _serviceProvider;
        private readonly IParserService _parserService;

        public DownloadsService(RedseatDbContext dbContext, ILogger<MonitorProgressService> logger, IServiceProvider serviceProvider, IParserService parserService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _parserService = parserService;
        }

        public Task<List<Download>> GetAllDownloads() => _dbContext.Downloads.AsQueryable().ToListAsync();
        public Task<List<RFile>> GetUnparsedFiles() => _dbContext.Files.AsQueryable().Where(f => !f.Parsed).ToListAsync();
        public Task<List<Download>> GetUnfiledFownloads() => _dbContext.Downloads.AsQueryable().Where(f => !f.FilesAvailable).ToListAsync();
        public Task<RFile> GetFile(int id) => _dbContext.Files.Include(nameof(RFile.Download)).SingleOrDefaultAsync(f => f.fileId == id);

    }

}