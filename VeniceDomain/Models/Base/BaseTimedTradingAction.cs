using System;
using VeniceDomain.Enums;

namespace VeniceDomain.Models.Base
{
    public abstract class BaseTimedTradingAction : BaseTradingAction
    {
        public CandlePeriod CandlePeriod { get; set; }

        public DateTime BeginDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
