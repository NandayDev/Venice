using VeniceDomain.Enums;
using VeniceDomain.Models.Base;

namespace VeniceDomain.Models
{
    public class TradingOrder : BaseTradingAction
    {
        public TradingOrder()
        {
        }

        public TradingOrder(FinancialInstrument financialInstrument, TradingOrderType orderType, int quantity, decimal? limitPrice = null)
        {
            FinancialInstrument = financialInstrument;
            OrderType = orderType;
            Quantity = quantity;
            LimitPrice = limitPrice;
        }

        public TradingOrderType OrderType { get; set; }

        public int Quantity { get; set; }

        public decimal? LimitPrice { get; set; }
    }
}
