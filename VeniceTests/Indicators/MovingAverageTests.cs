using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using VeniceDomain.Services.TechnicalAnalysis;
using Xunit;

namespace VeniceTests
{
    public class MovingAverageTests
    {
        [Fact]
        public void TestMovingAverage()
        {
            DataFetcher dataFetcher = new (CandlePeriod.DAILY);
            var candles = dataFetcher.Candles;

            int[] periods = Enumerable.Range(3, 20).ToArray();

            new SimpleMovingAverageCalc(candles, periods).GetAll();

            new WeightedMovingAverageCalc(candles, periods).GetAll();

            foreach(int period in periods)
            {
                AssertMovingAverageValue(candles, MovingAverageType.SIMPLE, period);
                AssertMovingAverageValue(candles, MovingAverageType.WEIGHTED, period);
            }
        }

        private static void AssertMovingAverageValue(List<CandleValue> candles, MovingAverageType movingAverageType, int period)
        {
            for (int i = period - 1; i < candles.Count; i++)
            {
                CandleValue candleToCheck = candles[i];
                decimal sum = 0;
                decimal average = 0;
                switch (movingAverageType)
                {
                    case MovingAverageType.SIMPLE:
                        for (int k = i; k > i - period; k--)
                        {
                            sum += candles[k].Close;
                        }
                        average = sum / period;
                        break;
                    case MovingAverageType.WEIGHTED:

                        for (int k = i, n = 0; k > i - period; k--, n++)
                        {
                            sum += candles[k].Close * (period - n); 
                        }
                        average = sum / (period * (period + 1) / 2m);
                        break;
                }
                Assert.Equal(average, candleToCheck.GetMovingAverage(movingAverageType, period).Value, 3);
            }
        }
    }
}
