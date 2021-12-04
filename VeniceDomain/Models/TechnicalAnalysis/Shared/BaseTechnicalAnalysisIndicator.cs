using VeniceDomain.Interfaces;

namespace VeniceDomain.Models.TechnicalAnalysis
{
    public abstract class BaseTechnicalAnalysisIndicator : ITechnicalAnalysisIndicator
    {
        public BaseTechnicalAnalysisIndicator() { }

        public BaseTechnicalAnalysisIndicator(CandleValue candleValue)
        {
            Candle = candleValue;
        }

        public virtual CandleValue Candle { get; set; }
    }
}
