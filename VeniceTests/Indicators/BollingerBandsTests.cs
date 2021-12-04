using System.Collections.Generic;
using System.Linq;
using VeniceDomain;
using VeniceDomain.Enums;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using VeniceDomain.Models.TechnicalAnalysis;
using VeniceDomain.Services.TechnicalAnalysis;
using Xunit;

namespace VeniceTests
{
    public class BollingerBandsTests
    {
        [Fact]
        public void TestBollingerBands()
        {
            DataFetcher dataFetcher = new(CandlePeriod.DAILY);
            
            foreach(int period in Enumerable.Range(3, 27))
            {
                foreach(int stdMultiplier in Enumerable.Range(1, 3))
                {
                    AssertBBValues(dataFetcher.Candles, period, stdMultiplier);
                }
            }
        }
        
        private void AssertBBValues(List<CandleValue> candles, int period, int stdMultiplier)
        {
            for (int i = period - 1; i < candles.Count; i++)
            {
                var candlesToConsider = candles
                    .Skip(i - period + 1)
                    .Take(period)
                    .Select(c => c.Close);

                /*
                 * BOLU=MA(TP,n)+m∗σ[TP,n]
                 * BOLD=MA(TP,n)−m∗σ[TP,n]
                 * where:
                 * BOLU=Upper Bollinger Band
                 * BOLD=Lower Bollinger Band
                 * MA=Moving average
                 * TP (typical price)=(High+Low+Close)÷3
                 * n=Number of days in smoothing period (typically 20)
                 * m=Number of standard deviations (typically 2)
                 * σ[TP,n]=Standard Deviation over last n periods of TP
                */

                decimal movingAverage = candlesToConsider.Average();
                decimal standardDeviation = candlesToConsider.GetStandardDeviation();
                decimal bolu = movingAverage + stdMultiplier * standardDeviation;
                decimal bold = movingAverage - stdMultiplier * standardDeviation;
                decimal bbw = bolu - bold;

                new BollingerBandCalc(candles, period, stdMultiplier)
                    .GetAll();

                BollingerBand bollingerBand = candles[i].GetBollingerBand(period, stdMultiplier);
                Assert.Equal(movingAverage, bollingerBand.MiddleAverageValue, 3);
                Assert.Equal(bolu, bollingerBand.UpperBandValue, 3);
                Assert.Equal(bold, bollingerBand.LowerBandValue, 3);
                Assert.Equal(bbw, bollingerBand.BollingerBandWidth, 3);
            }

            //Enumerable.Range(3, 20);
        }
    }
}
