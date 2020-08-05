using System.Threading.Tasks;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using RedSeatServer.Downloaders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using TvDbSharper;
using AutoMapper;
using TvDbSharper.Dto;
using System.Linq;
using System.Threading;

namespace RedSeatServer.Services
{
    public interface IShowService
    {
        Task<Show> getShow(int id);
        Task<Show> getShowByTvdbId(int tvdbId);
        Task<int> tvdbIdByTitle(string title);
    }

    public class ShowService : IShowService
    {
        private RedseatDbContext _dbContext;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        static SemaphoreSlim _showAddLock = new SemaphoreSlim(1, 1);

        public ShowService(RedseatDbContext dbContext, ILogger<MonitorProgressService> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
        }

        public Task<Show> getShow(int id)
        {
            return _dbContext.Shows.Include(b => b.Episodes).SingleOrDefaultAsync(s => s.ShowId == id);
        }

        public async Task<int> tvdbIdByTitle(string title)
        {
            
            var client = new TvDbClient();
            await client.Authentication.AuthenticateAsync("F6EE96D0B5484A59");


            var showTvDbSearch = (await client.Search.SearchSeriesByNameAsync(title)).Data;
           return showTvDbSearch.First().Id;
            

        }

        public async Task<Show> getShowByTvdbId(int tvdbId)
        {
            await _showAddLock.WaitAsync();
            try
            {
            var savedShow = await _dbContext.Shows.Include(b => b.Episodes).SingleOrDefaultAsync(s => s.TvdbId == tvdbId);
            if (savedShow != null)
                return savedShow;

            var client = new TvDbClient();
            await client.Authentication.AuthenticateAsync("F6EE96D0B5484A59");


            var showTvDb = (await client.Series.GetAsync(tvdbId)).Data;
            var show = _mapper.Map<Show>(showTvDb);


            var tasks = new List<Task<TvDbResponse<EpisodeRecord[]>>>();


            var firstResponse = await client.Series.GetEpisodesAsync(tvdbId, 1);

            for (int i = 2; i <= firstResponse.Links.Last; i++)
            {
                tasks.Add(client.Series.GetEpisodesAsync(tvdbId, i));
            }

            var results = await Task.WhenAll(tasks);

            var episodesTvDb = firstResponse.Data.Concat(results.SelectMany(x => x.Data));
            var episodes = _mapper.Map<List<Episode>>(episodesTvDb);

            show.Episodes = episodes;

            _dbContext.Shows.Add(show);
            _dbContext.SaveChanges();
            return show;
            } finally {
                _showAddLock.Release();
            }

        }
    }

}