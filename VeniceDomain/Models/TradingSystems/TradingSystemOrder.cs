using VeniceDomain.Enums;
using VeniceDomain.Models.Base;

namespace VeniceDomain.Models
{
    public class TradingSystemOrder
    {
        public TradingSystemOrder(FinancialInstrument financialInstrument, TradingOrderType orderType, CandleValue candleThatMeetsConditions, int candleThatMeetsConditionIndex)
        {
            FinancialInstrument = financialInstrument;
            OrderType = orderType;
            CandleThatMeetsConditions = candleThatMeetsConditions;
            CandleThatMeetsConditionIndex = candleThatMeetsConditionIndex;
        }

        public TradingSystemOrder(FinancialInstrument financialInstrument, TradingOrderType orderType, CandleValue candleThatMeetsConditions, int candleThatMeetsConditionIndex, 
            decimal? takeProfitPercentage = null, decimal? stopLossPercentage = null, decimal? trailingStopPercentage = null, bool flatAtNextClose = false,
            decimal quantityPercentage = 1)
            : this(financialInstrument, orderType, candleThatMeetsConditions, candleThatMeetsConditionIndex)
        {
            TakeProfitPercentage = takeProfitPercentage;
            StopLossPercentage = stopLossPercentage;
            TrailingStopPrice = trailingStopPercentage;
            FlatAtNextClose = flatAtNextClose;
            QuantityPercentage = quantityPercentage;
        }

        public FinancialInstrument FinancialInstrument { get; }

        public TradingOrderType OrderType { get; }

        /// <summary>
        /// The candle that meets the condition of the trading system<br/>
        /// This usually means this is NOT the candle to consider for the order, instead the system goes long/short on the very next one
        /// </summary>
        public CandleValue CandleThatMeetsConditions { get; }

        /// <summary>
        /// Index of <see cref="CandleThatMeetsConditions"/>
        /// </summary>
        public int CandleThatMeetsConditionIndex { get; }

        public decimal? TakeProfitPercentage { get; }

        public decimal? StopLossPercentage { get; }

        public decimal? TrailingStopPrice { get; set; }

        public bool FlatAtNextClose { get; }

        public decimal QuantityPercentage { get; } = 1;

        public override string ToString()
        {
            return string.Format("{0} {1}", OrderType.ToString(), FinancialInstrument?.Ticker ?? "NULL");
        }
    }
}
