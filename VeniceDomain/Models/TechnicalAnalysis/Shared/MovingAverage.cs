using VeniceDomain.Enums;

namespace VeniceDomain.Models.TechnicalAnalysis
{
    public abstract class MovingAverage : BaseTechnicalAnalysisIndicator
    {
        public MovingAverage() : base()
        {
        }

        public MovingAverage(CandleValue candle, decimal value, int period, CandleValueElement elementOfAverage = CandleValueElement.Close) : base(candle)
        {
            CandleValueElement = elementOfAverage;
            Value = value;
            Period = period;
        }

        public decimal Value { get; set; }

        public int Period { get; set; }

        public CandleValueElement CandleValueElement { get; set; }

        public override string ToString()
        {
            return $"MA period {Period} value {Value}";
        }
    }
}
