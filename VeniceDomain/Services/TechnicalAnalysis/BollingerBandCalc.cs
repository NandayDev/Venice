using System;
using System.Collections.Generic;
using VeniceDomain.Models;
using VeniceDomain.Models.TechnicalAnalysis;

namespace VeniceDomain.Services.TechnicalAnalysis
{
    public class BollingerBandCalc : BaseIndicatorCalc<BollingerBand>
    {
        public BollingerBandCalc(IEnumerable<CandleValue> candles, int period, decimal standardDeviationMultiplier) : base(candles)
        {
            this.period = period;
            standardDevMultiplier = standardDeviationMultiplier;
            indicatorKey = BollingerBand.GetIndicatorString(period, standardDeviationMultiplier);
        }

        private readonly int period;
        private readonly decimal standardDevMultiplier;
        private readonly string indicatorKey;

        public override void GetAll()
        {
            for (int i = 0; i < Candles.Count; i++)
            {
                CandleValue candle = Candles[i];
                if (i >= period - 1)
                {
                    // Gets the last pertinent closes //
                    int startingIndex = i - period + 1;
                    decimal middleAverage = 0;
                    for (int k = startingIndex; k < startingIndex + period; k++)
                    {
                        middleAverage += Candles[k].Close;
                    }
                    middleAverage /= period;

                    decimal sum = 0;
                    for (int k = startingIndex; k < startingIndex + period; k++)
                    {
                        decimal close = Candles[k].Close;
                        sum += (close - middleAverage) * (close - middleAverage);
                    }

                    decimal multipliedStandardDeviation = standardDevMultiplier * (decimal)Math.Sqrt((double)(sum / (period - 1)));
                    decimal upperBand = middleAverage + multipliedStandardDeviation;
                    decimal lowerBand = middleAverage - multipliedStandardDeviation;
                    var bollingerBand = new BollingerBand(candle, period, standardDevMultiplier, upperBand, middleAverage, lowerBand);
                    candle.AddIndicator(indicatorKey, bollingerBand);
                }
                else
                    candle.AddIndicator(indicatorKey, new BollingerBand(candle, period, standardDevMultiplier, 0, 0, 0));
            }
        }
    }
}
