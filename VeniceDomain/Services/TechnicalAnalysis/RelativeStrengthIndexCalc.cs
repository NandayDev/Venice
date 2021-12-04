using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using VeniceDomain.Models.Exceptions;
using VeniceDomain.Models.TechnicalAnalysis;
using VeniceDomain.Models.TechnicalAnalysis.Shared;

namespace VeniceDomain.Services.TechnicalAnalysis
{
    public class RelativeStrengthIndexCalc : BaseIndicatorCalc<RelativeStrengthIndex>
    {
        public RelativeStrengthIndexCalc(IEnumerable<CandleValue> candles, int period)
            : base(candles)
        {
            if (Candles.Count() < period)
                throw new TechnicalAnalysisException("Can't calculate RSI: candles list size is too small (must be >= of period)");
            _period = period;
            _indicatorKey = RelativeStrengthIndex.GetIndicatorString(period);
        }

        private readonly int _period;
        private readonly string _indicatorKey;
        private FixedSizedList<decimal> positiveValues;
        private FixedSizedList<decimal> negativeValues;

        public override void GetAll()
        {
            positiveValues = new FixedSizedList<decimal>(_period);
            negativeValues = new FixedSizedList<decimal>(_period);
            decimal? pastAverageGain = null;
            decimal? pastAverageLoss = null;
            // First list is average gains
            // Second list is average losses //
            //var averages = new FixedSizedDoubleList<decimal>(_period);
            for (int i = 1; i < Candles.Count; i++)
            {
                CandleValue candle = Candles[i];
                CandleValue previousCandle = Candles[i - 1];
                // Starting from the first element (i = 1), we calculate gains and losses, and add them to the lists //
                decimal differenceFromDayBefore = candle.Close - previousCandle.Close;
                // Elements before period int are automatically removed with this method //
                if (differenceFromDayBefore > 0)
                {
                    positiveValues.Add(differenceFromDayBefore);
                }
                else if (differenceFromDayBefore < 0)
                {
                    negativeValues.Add(differenceFromDayBefore);
                }

                decimal rsiValue = 0;

                if (i > _period - 1)
                {
                    decimal averageGain = 0;
                    decimal averageLoss = 0;
                    if (pastAverageGain == null || pastAverageLoss == null)
                    {
                        foreach (var value in positiveValues)
                        {
                            averageGain += value;
                        }
                        averageGain /= _period;

                        foreach (var value in negativeValues)
                        {
                            averageLoss += value;
                        }
                        averageLoss = Math.Abs(averageLoss / _period);
                    }
                    else
                    {
                        averageGain = ((pastAverageGain.Value * (_period - 1)) + (differenceFromDayBefore > 0 ? differenceFromDayBefore : 0m)) / _period;
                        averageLoss = ((pastAverageLoss.Value * (_period - 1)) + (differenceFromDayBefore < 0 ? -differenceFromDayBefore : 0m)) / _period;
                    }

                    if (averageLoss == 0)
                    {
                        rsiValue = 100M;
                    }
                    else
                    {
                        decimal rs = averageGain / averageLoss;
                        rsiValue = 100M - (100M / (1M + rs));
                    }
                    pastAverageGain = averageGain;
                    pastAverageLoss = averageLoss;
                }
                RelativeStrengthIndex rsi = new RelativeStrengthIndex(candle, rsiValue, _period);
                candle.AddIndicator(_indicatorKey, rsi);
            }
            
        }
    }
}
