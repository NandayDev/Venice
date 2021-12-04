using VeniceDomain.Enums;

namespace VeniceDomain.Models.TechnicalAnalysis
{
    public class WeightedMovingAverage : MovingAverage
    {
        public WeightedMovingAverage() : base()
        {
        }

        public WeightedMovingAverage(CandleValue candle, decimal value, int period, CandleValueElement elementOfAverage = CandleValueElement.Close) 
            : base(candle, value, period, elementOfAverage)
        {
        }

        /// <summary>
        /// Returns a string to retrieve this indicator in the <see cref="CandleValue.Indicators"/> dictionary
        /// </summary>
        internal static string GetIndicatorString(int period, CandleValueElement elementOfAverage = CandleValueElement.Close)
            => "WeightedMovingAverage" + period + elementOfAverage.ToString();
    }
}
