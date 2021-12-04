using System;

namespace VeniceDomain.Enums
{
    public enum CandlePeriod : byte
    {
        TICK_BY_TICK = 0,
        ONE_MINUTE = 1,
        TWO_MINUTES = 2,
        FIVE_MINUTES = 3,
        FIFTEEN_MINUTES = 4,
        HALF_HOUR = 5,
        ONE_HOUR = 6,
        FOUR_HOURS = 7,
        DAILY = 8,
        WEEKLY = 9,
        MONTHLY = 10
    }

    public static class CandlePeriodExtension
    {
        public static int ConvertToSeconds(this CandlePeriod candlePeriod)
        {
            return candlePeriod switch
            {
                CandlePeriod.TICK_BY_TICK => 1,//todo ???
                CandlePeriod.ONE_MINUTE => 60,
                CandlePeriod.TWO_MINUTES => CandlePeriod.ONE_MINUTE.ConvertToSeconds() * 2,
                CandlePeriod.FIVE_MINUTES => CandlePeriod.ONE_MINUTE.ConvertToSeconds() * 5,
                CandlePeriod.FIFTEEN_MINUTES => CandlePeriod.FIVE_MINUTES.ConvertToSeconds() * 3,
                CandlePeriod.HALF_HOUR => CandlePeriod.FIFTEEN_MINUTES.ConvertToSeconds() * 2,
                CandlePeriod.ONE_HOUR => CandlePeriod.HALF_HOUR.ConvertToSeconds() * 2,
                CandlePeriod.FOUR_HOURS => CandlePeriod.ONE_HOUR.ConvertToSeconds() * 4,
                CandlePeriod.DAILY => CandlePeriod.ONE_HOUR.ConvertToSeconds() * 24,
                CandlePeriod.WEEKLY => CandlePeriod.DAILY.ConvertToSeconds() * 5,
                _ => throw new ArgumentException("Add missing case!"),
            };
        }
    }
}
