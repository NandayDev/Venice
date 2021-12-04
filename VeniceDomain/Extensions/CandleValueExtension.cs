using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Models;
using VeniceDomain.Models.TechnicalAnalysis;

namespace VeniceDomain.Extensions
{
    public static class CandleValueExtension
    {
        public static decimal GetPercentageDifferenceInClose(this CandleValue candle, CandleValue previousCandle)
        {
            decimal previousClose = previousCandle.Close;
            decimal actualClose = candle.Close;
            return (actualClose / previousClose) - 1M;
        }

        public static decimal GetMarkToMarketValue(this CandleValue candle, IEnumerable<Transaction> transactions)
        {
            int quantity = transactions.GetNetQuantity();
            return quantity * candle.Close;
        }
    }
}
