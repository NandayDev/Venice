using VeniceDomain.Enums;
using VeniceDomain.Models.Base;

namespace VeniceDomain.Models
{
    public class TradingSystemOrder
    {
        public TradingSystemOrder(FinancialInstrument financialInstrument, TradingOrderType orderType, CandleValue candleThatMeetsConditions)
        {
            FinancialInstrument = financialInstrument;
            OrderType = orderType;
            CandleThatMeetsConditions = candleThatMeetsConditions;
        }

        public TradingSystemOrder(FinancialInstrument financialInstrument, TradingOrderType orderType, CandleValue candleThatMeetsConditions, 
            decimal? takeProfitPercentage = null, decimal? stopLossPercentage = null, decimal? trailingStopPercentage = null, bool flatAtNextClose = false)
            : this(financialInstrument, orderType, candleThatMeetsConditions)
        {
            TakeProfitPrice = takeProfitPercentage;
            StopLossPrice = stopLossPercentage;
            TrailingStopPrice = trailingStopPercentage;
            FlatAtNextClose = flatAtNextClose;
        }

        public FinancialInstrument FinancialInstrument { get; }

        public TradingOrderType OrderType { get; }

        /// <summary>
        /// The candle that meets the condition of the trading system<br/>
        /// This usually means this is NOT the candle to consider for the order, instead the system goes long/short on the very next one
        /// </summary>
        public CandleValue CandleThatMeetsConditions { get; }

        public decimal? TakeProfitPrice { get; }

        public decimal? StopLossPrice { get; }

        public decimal? TrailingStopPrice { get; }

        public bool FlatAtNextClose { get; }

        public override string ToString()
        {
            return string.Format("{0} {1}", OrderType.ToString(), FinancialInstrument?.Ticker ?? "NULL");
        }
    }
}
