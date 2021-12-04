using VeniceDomain.Models;

namespace VeniceDomain.Interfaces
{
    public interface ITechnicalAnalysisIndicator
    {
        CandleValue Candle { get; }
    }
}
