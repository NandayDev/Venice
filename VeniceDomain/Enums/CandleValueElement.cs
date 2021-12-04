using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Models;

namespace VeniceDomain.Enums
{
    public enum CandleValueElement
    {
        Open,
        High,
        Low,
        Close
    }

    public static class CandleValueElementExtension
    {
        public static decimal GetCandleElementValue(this CandleValueElement candleElement, CandleValue candleValue)
        {
            return candleElement switch
            {
                CandleValueElement.Open => candleValue.Open,
                CandleValueElement.High => candleValue.High,
                CandleValueElement.Low => candleValue.Low,
                _ => candleValue.Close,
            };
        }

        public static IEnumerable<decimal> GetCandleValues(this CandleValueElement candleValueElement, IEnumerable<CandleValue> candleValues)
        {
            return candleValues.Select(c => candleValueElement.GetCandleElementValue(c));
        }
    }
}
