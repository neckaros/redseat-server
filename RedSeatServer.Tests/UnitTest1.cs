using System;
using Xunit;
using RedSeatServer.Services;
using Xunit.Abstractions;

namespace RedSeatServer.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

    public UnitTest1(ITestOutputHelper output)
    {
        this.output = output;
    }

        [Fact]
        public void Test1()
        {
            var filename = "Brooklyn.Nine-Nine.S07E06.1080p.AMZN.WEB-DL.DDP5.1.H.264-NTb.mkv";
            var info = Parser.ParseTitle(filename);
            Assert.Equal(7, info.SeasonNumber);
            output.WriteLine(info.ToString());
            output.WriteLine("'test'");
            Assert.Equal("Brooklyn Nine-Nine", info.SeriesTitle);
            Console.WriteLine(info);
        }
    }
}
