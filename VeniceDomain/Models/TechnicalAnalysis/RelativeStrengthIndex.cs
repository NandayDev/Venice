using VeniceDomain.Interfaces;
using VeniceDomain.Models.TechnicalAnalysis.Shared;

namespace VeniceDomain.Models.TechnicalAnalysis
{
    public class RelativeStrengthIndex : BaseTechnicalAnalysisIndicator
    {
        public RelativeStrengthIndex() : base()
        {
        }

        public RelativeStrengthIndex(CandleValue candle, decimal value, int period) : base(candle)
        {
            Value = value;
            Period = period;
        }

        public decimal Value { get; }

        public int Period { get; }

        /// <summary>
        /// Returns a string to retrieve this indicator in the <see cref="CandleValue.Indicators"/> dictionary
        /// </summary>
        internal static string GetIndicatorString(int period)
            => "RelativeStrengthIndex" + period;

        public override string ToString()
        {
            return $"RSI period {Period} value {Value}";
        }
    }
}
