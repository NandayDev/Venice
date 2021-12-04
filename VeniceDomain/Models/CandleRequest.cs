using System;
using VeniceDomain.Enums;
using VeniceDomain.Models.Base;

namespace VeniceDomain.Models
{
    public class CandleRequest : BaseTimedTradingAction
    {
        public CandleRequest(FinancialInstrument financialInstrument, CandlePeriod candlePeriod, DateTime beginDate, DateTime endDate)
        {
            FinancialInstrument = financialInstrument;
            CandlePeriod = candlePeriod;
            BeginDate = beginDate;
            EndDate = endDate;
        }
    }
}
