using System.Collections.Generic;
using System.Threading.Tasks;
using VeniceDomain.Enums;
using VeniceDomain.Models;
using VeniceDomain.Models.Exceptions;
using VeniceDomain.Models.TechnicalAnalysis;
using VeniceDomain.Services.TechnicalAnalysis;

namespace VeniceDomain.Extensions
{
    public static class CandleValueIndicatorExtension
    {
        public static MovingAverage? GetMovingAverage(this CandleValue candleValue, MovingAverageType movingAverageType, int period, IEnumerable<CandleValue>? candles = null)
        {
            var indicatorKey = movingAverageType switch
            {
                MovingAverageType.SIMPLE => SimpleMovingAverage.GetIndicatorString(period),
                MovingAverageType.WEIGHTED => WeightedMovingAverage.GetIndicatorString(period),
                _ => throw new System.Exception(),
            };

            MovingAverage? average = candleValue.GetIndicator<MovingAverage>(indicatorKey);

            if (average == null && candles != null)
            {
                switch (movingAverageType)
                {
                    case MovingAverageType.SIMPLE:
                        using (var service = new SimpleMovingAverageCalc(candles, period))
                            service.GetAll();
                        break;
                    case MovingAverageType.WEIGHTED:
                        using (var service = new WeightedMovingAverageCalc(candles, period))
                            service.GetAll();
                        break;
                    default:
                        throw new System.Exception();
                }
                average = candleValue.GetIndicator<MovingAverage>(indicatorKey);
            }
            if (average?.Value == 0)
                return null;
            return average;
        }

        public static void GetMultipleMovingAverage(this IEnumerable<CandleValue> candles, MovingAverageType movingAverageType, params int[] periods)
        {
            switch (movingAverageType)
            {
                case MovingAverageType.SIMPLE:
                    using (var service = new SimpleMovingAverageCalc(candles, periods))
                        service.GetAll();
                    break;

                case MovingAverageType.WEIGHTED:
                    using (var service = new WeightedMovingAverageCalc(candles, periods))
                        service.GetAll();
                    break;
            }
        }

        public static Task GetMultipleMovingAverageAsync(this IEnumerable<CandleValue> candles, MovingAverageType movingAverageType, params int[] periods)
        {
            switch (movingAverageType)
            {
                case MovingAverageType.SIMPLE:
                    using (var service = new SimpleMovingAverageCalc(candles, periods))
                        return service.GetAllAsync();

                case MovingAverageType.WEIGHTED:
                    using (var service = new WeightedMovingAverageCalc(candles, periods))
                        return service.GetAllAsync();
            }
            throw new System.Exception();
        }

        public static BollingerBand? GetBollingerBand(this CandleValue candleValue, int period, decimal standardDeviationMultiplier, IEnumerable<CandleValue>? candles = null)
        {
            string indicatorKey = BollingerBand.GetIndicatorString(period, standardDeviationMultiplier);
            var bollingerBand = candleValue.GetIndicator<BollingerBand>(indicatorKey);
            if (bollingerBand == null && candles != null)
            {
                using var service = new BollingerBandCalc(candles, period, standardDeviationMultiplier);
                service.GetAll();
                bollingerBand = candleValue.GetIndicator<BollingerBand>(indicatorKey);
            }
            return bollingerBand?.UpperBandValue == 0 ? null : bollingerBand;
        }

        public static void GetBollingerBand(this IEnumerable<CandleValue> candles, int period, decimal standardDeviationMultiplier)
        {
            using var service = new BollingerBandCalc(candles, period, standardDeviationMultiplier);
            service.GetAll();
        }

        public static RelativeStrengthIndex GetRelativeStrengthIndex(this CandleValue candleValue, int period, IEnumerable<CandleValue>? candles = null)
        {
            string indicatorKey = RelativeStrengthIndex.GetIndicatorString(period);
            RelativeStrengthIndex relativeStrengthIndex = candleValue.GetIndicator<RelativeStrengthIndex>(indicatorKey);
            if (relativeStrengthIndex == null && candles != null)
            {
                try
                {
                    using var service = new RelativeStrengthIndexCalc(candles, period);
                    service.GetAll();
                    relativeStrengthIndex = candleValue.GetIndicator<RelativeStrengthIndex>(indicatorKey);
                }
                catch (TechnicalAnalysisException)
                {
                }
            }
            return relativeStrengthIndex?.Value == 0 ? null : relativeStrengthIndex;
        }

        public static void GetRelativeStrengthIndex(this IEnumerable<CandleValue> candleValues, int period)
        {
            using var service = new RelativeStrengthIndexCalc(candleValues, period);
            service.GetAll();
        }
    }
}
