using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedSeatServer.Models;
using Microsoft.EntityFrameworkCore;
using RedSeatServer.Services;
using TvDbSharper;
using AutoMapper;
using TvDbSharper.Dto;

namespace RedSeatServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowController : ControllerBase
    {
        private readonly ILogger<ShowController> _logger;
        private readonly IIoService _ioService;
        private readonly IMapper _mapper;
        private readonly IShowService _showService;

        public ShowController(ILogger<ShowController> logger, IIoService ioService, IMapper mapper, IShowService showService)
        {
            _logger = logger;
            _ioService = ioService;
            _mapper = mapper;
            _showService = showService;
        }

        [HttpGet]
        public async Task<ActionResult<Show>> Get(int tvdbid)
        {

            var show = await _showService.getShowByTvdbId(tvdbid);
            return show;
        }

        [HttpGet("{id}/episode")]
        public async Task<ActionResult<IEnumerable<EpisodeRecord>>> GetEpisodes(int id)
        {
            var client = new TvDbClient();
            await client.Authentication.AuthenticateAsync("F6EE96D0B5484A59");

            var tasks = new List<Task<TvDbResponse<EpisodeRecord[]>>>();

            var firstResponse = await client.Series.GetEpisodesAsync(id, 1);

            for (int i = 2; i <= firstResponse.Links.Last; i++)
            {
                tasks.Add(client.Series.GetEpisodesAsync(id, i));
            }

            var results = await Task.WhenAll(tasks);

            var episodesTvDb = firstResponse.Data.Concat(results.SelectMany(x => x.Data));
            var episodes = _mapper.Map<List<Episode>>(episodesTvDb);
            return Ok(episodes);
        }

    }
}
