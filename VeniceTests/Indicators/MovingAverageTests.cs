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

            //for (int i = 4; i < candles.Count; i++)
            //{
            //    var sma5 = Math.Round(candles[i].GetMovingAverage(MovingAverageType.SIMPLE, 5).Value, 6);
            //    File.AppendAllText("test.txt", $"{i} - SMA_5: {sma5}");
            //    var wma5 = Math.Round(candles[i].GetMovingAverage(MovingAverageType.WEIGHTED, 5).Value, 6);
            //    File.AppendAllText("test.txt", $" - WMA_5: {wma5}");
            //    if (i >= 19)
            //    {
            //        var sma20 = Math.Round(candles[i].GetMovingAverage(MovingAverageType.SIMPLE, 20).Value, 6);
            //        File.AppendAllText("test.txt", $"{i} - SMA_20: {sma20}");
            //        var wma20 = Math.Round(candles[i].GetMovingAverage(MovingAverageType.WEIGHTED, 20).Value, 6);
            //        File.AppendAllText("test.txt", $" - WMA_20: {wma20}");
            //    }
            //    File.AppendAllText("test.txt", "\n");
            //}
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
