using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Models;

namespace VeniceDomain.Services
{
    /// <summary>
    /// Service class to compress a list of <see cref="CandleValue"/> with a lower <see cref="CandlePeriod"/> to a higher one
    /// </summary>
    public static class CandleCompressor
    {
        public static List<CandleValue> Compress(List<CandleValue> candles, CandlePeriod targetPeriod)
        {
            return CompressInternal(candles, targetPeriod).ToList();
        }

        private static IEnumerable<CandleValue> CompressInternal(IEnumerable<CandleValue> candles, CandlePeriod targetPeriod)
        {
            if (candles == null || !candles.Any())
            {
                yield break;
            }

            TimeSpan targetPeriodTimespan = TimeSpan.FromSeconds(targetPeriod.ConvertToSeconds());

            DateTime nextCandleStartDate;
            decimal currentHigh;
            decimal currentLow;
            int currentVolume;

            CandleValue currentCandle = candles.First();

            StartWithNewCandle(currentCandle);
            
            foreach(CandleValue candle in candles.Skip(1))
            {
                if (candle.StartDate >= nextCandleStartDate)
                {
                    // Current candle exceeds the bounds of the date //
                    CompleteCurrentCandle();
                    yield return currentCandle;
                    StartWithNewCandle(candle);
                }
                else
                {
                    currentHigh = Math.Max(currentHigh, candle.High);
                    currentLow = Math.Min(currentLow, candle.Low);
                    currentVolume += candle.Volume;
                    currentCandle.Close = candle.Close;
                }
            }

            CompleteCurrentCandle();
            yield return currentCandle;

            void StartWithNewCandle(CandleValue candle)
            {
                currentCandle = candle;
                nextCandleStartDate = currentCandle.StartDate + targetPeriodTimespan;
                currentHigh = currentCandle.High;
                currentLow = currentCandle.Low;
                currentVolume = currentCandle.Volume;
            }

            void CompleteCurrentCandle()
            {
                currentCandle.CandlePeriod = targetPeriod;
                currentCandle.High = currentHigh;
                currentCandle.Low = currentLow;
                currentCandle.Volume = currentVolume;
            }
        }
    }
}
