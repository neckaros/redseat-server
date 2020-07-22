using System;
using Xunit;
using RedSeatServer.Services;
using Xunit.Abstractions;
using RedSeatServer.Services.Parser;

namespace RedSeatServer.Tests
{
    public class ParserServiceTest
    {
        private readonly ITestOutputHelper output;
        private readonly XunitLogger<ParserService> logger;

    public ParserServiceTest(ITestOutputHelper output)
    {
        this.output = output;
        this.logger = new XunitLogger<ParserService>(output);
    }

        [Fact]
        public void Test1()
        {
            var parserService = new ParserService(logger);
            var filename = "Brooklyn.Nine-Nine.S07E06.1080p.AMZN.WEB-DL.DDP5.1.H.264-NTb.mkv";
            var info = parserService.ParseTitle(filename);
            Assert.Equal(7, info.SeasonNumber);
            Assert.Equal("Brooklyn Nine-Nine", info.SeriesTitle);
            
            Assert.Equal(Quality.WEBDL1080p, info.Quality.Quality);
            Console.WriteLine(info);
        }
    }
}
