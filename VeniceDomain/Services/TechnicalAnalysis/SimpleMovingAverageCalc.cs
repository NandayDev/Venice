using System.Collections.Generic;
using VeniceDomain.Enums;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using VeniceDomain.Models.TechnicalAnalysis;

namespace VeniceDomain.Services.TechnicalAnalysis
{
    public class SimpleMovingAverageCalc : BaseIndicatorCalc<SimpleMovingAverage>
    {
        public SimpleMovingAverageCalc(IEnumerable<CandleValue> candles, CandleValueElement candleElement = CandleValueElement.Close, params int[] periods)
            : base(candles)
        {
            _candleValueElement = candleElement;
            _periods = periods;
        }

        public SimpleMovingAverageCalc(IEnumerable<CandleValue> candles, params int[] periods) : this(candles, CandleValueElement.Close, periods)
        {
        }

        protected readonly int[] _periods;

        protected readonly CandleValueElement _candleValueElement;

        public override void GetAll()
        {
            for (int i = 0; i < Candles.Count; i++)
            {
                CandleValue candle = Candles[i];
                foreach (int period in _periods)
                {
                    string indicatorKey = SimpleMovingAverage.GetIndicatorString(period, _candleValueElement);
                    if (i >= period - 1)
                    {
                        decimal average = 0;
                        int periodSum = 0;
                        for (int k = i - period + 1; k <= i; k++)
                        {
                            average += _candleValueElement.GetCandleElementValue(Candles[k]);
                            periodSum += 1;
                        }
                        average /= periodSum;

                        //decimal average = Candles.Skip(i - period + 1).Take(period - 1)
                        //    .Select(c => CandleValueElement.GetCandleElementValue(c))
                        //    .Average();
                        var movingAverage = new SimpleMovingAverage(candle, average, period);
                        candle.AddIndicator(indicatorKey, movingAverage);
                    }
                    else
                        candle.AddIndicator(indicatorKey, new SimpleMovingAverage(candle, 0, period));
                }
            }
        }
    }
}
