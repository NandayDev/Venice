using System.Collections.Generic;
using VeniceDomain.Enums;
using VeniceDomain.Models;
using VeniceDomain.Models.TechnicalAnalysis;

namespace VeniceDomain.Services.TechnicalAnalysis
{
    public class WeightedMovingAverageCalc : SimpleMovingAverageCalc
    {
        public WeightedMovingAverageCalc(IEnumerable<CandleValue> candles, params int[] periods)
            : base(candles, periods)
        {
        }

        public WeightedMovingAverageCalc(IEnumerable<CandleValue> candles, CandleValueElement candleElement = CandleValueElement.Close, params int[] periods)
            : base(candles, candleElement, periods)
        {
        }

        public override void GetAll()
        {
            for (int i = 0; i < Candles.Count; i++)
            {
                CandleValue candle = Candles[i];
                foreach (var period in _periods)
                {
                    string indicatorKey = WeightedMovingAverage.GetIndicatorString(period, _candleValueElement);
                    if (i >= period - 1)
                    {
                        // Gets the last pertinent closes //
                        decimal weightedAverage = 0;
                        for (int n = i, k = 0; n >= i - period + 1; n--, k++)
                        {
                            weightedAverage += _candleValueElement.GetCandleElementValue(Candles[n]) * (period - k);
                        }
                        weightedAverage /= ((period * (period + 1)) / 2.0m);
                        var movingAverage = new WeightedMovingAverage(candle, weightedAverage, period);
                        candle.AddIndicator(indicatorKey, movingAverage);
                    }
                    else
                        candle.AddIndicator(indicatorKey, new WeightedMovingAverage(candle, 0, period));
                }
            }
        }
    }
}
