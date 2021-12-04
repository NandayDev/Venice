using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain.Models
{
    public class TradingQueryResult
    {
        public TradingQueryResult(DateTime date)
        {
            Date = date;
        }

        public DateTime Date { get; set; }

        public Dictionary<FinancialInstrument, decimal> NetEarnings { get; internal set; } = new Dictionary<FinancialInstrument, decimal>();

        public Dictionary<FinancialInstrument, decimal> NetPercentEarnings { get; internal set; } = new Dictionary<FinancialInstrument, decimal>();

        public Dictionary<FinancialInstrument, decimal> CounterValues { get; internal set; } = new Dictionary<FinancialInstrument, decimal>();

        public decimal GetTotalNetBalance()
        {
            decimal total = 0;
            foreach (var cv in NetEarnings)
                total += cv.Value;
            return total;
        }

        public decimal GetTotalCounterValue()
        {
            decimal total = 0;
            foreach (var cv in CounterValues)
                total += cv.Value;
            return total;
        }

        public decimal GetTotalPercentNetEarning()
        {
            var totalCounterValue = GetTotalCounterValue();
            if (totalCounterValue != 0)
                return GetTotalNetBalance() / GetTotalCounterValue() * 100M;
            return 0;
        }
    }
}
