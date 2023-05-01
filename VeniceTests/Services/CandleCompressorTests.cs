using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Models;
using VeniceDomain.Services;
using Xunit;

namespace VeniceTests.Services
{
    public class CandleCompressorTests
    {
        [Fact]
        public void InvalidInputs()
        {
            IEnumerable<CandleValue> result = CandleCompressor.Compress(null, CandlePeriod.DAILY);
            Assert.Empty(result);
            result = CandleCompressor.Compress(new List<CandleValue>(), CandlePeriod.DAILY);
            Assert.Empty(result);
        }

        [Fact]
        public void CompressOneMinuteToFive()
        {
            List<CandleValue> candles = new DataFetcher(CandlePeriod.ONE_MINUTE).Candles.Take(10).ToList();
            foreach (CandleValue candle in candles)
            {
                candle.Volume = 100;
            }

            List<CandleValue> result = CandleCompressor.Compress(candles, CandlePeriod.FIVE_MINUTES).ToList();

            Assert.Equal(2, result.Count);
            CandleValue firstCandle = result[0];
            CandleValue secondCandle = result[^1];

            Assert.Equal(12.268M, firstCandle.Close, 3);
            Assert.Equal(11.448M, firstCandle.Open, 3);
            Assert.Equal(12.384M, firstCandle.High, 3);
            Assert.Equal(11.154M, firstCandle.Low, 3);
            Assert.Equal(500, firstCandle.Volume);

            Assert.Equal(12.584M, secondCandle.Close, 3);
            Assert.Equal(12.25M, secondCandle.Open, 3);
            Assert.Equal(12.63M, secondCandle.High, 3);
            Assert.Equal(12.136M, secondCandle.Low, 3);
            Assert.Equal(500, secondCandle.Volume);
        }
    }
}
