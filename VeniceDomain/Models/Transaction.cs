using System;
using VeniceDomain.Enums;

namespace VeniceDomain.Models
{
    public class Transaction
    {
        public Transaction()
        {
        }

        public Transaction(FinancialInstrument financialInstrument, decimal executionPrice, DateTime executionTime, TradingOrder? originalOrder,
            TradingOrderType orderType, int quantity)
        {
            FinancialInstrument = financialInstrument;
            OrderType = orderType;
            Quantity = quantity;
            ExecutionPrice = executionPrice;
            ExecutionTime = executionTime;
            OriginalOrder = originalOrder;
        }

        public Transaction(FinancialInstrument financialInstrument, decimal executionPrice, DateTime executionTime, TradingOrder? originalOrder,
            TradingOrderType orderType, int quantity, bool isStopLoss, bool isTakeProfit)
        {
            FinancialInstrument = financialInstrument;
            OrderType = orderType;
            Quantity = quantity;
            ExecutionPrice = executionPrice;
            ExecutionTime = executionTime;
            OriginalOrder = originalOrder;
            IsStopLoss = isStopLoss;
            IsTakeProfit = isTakeProfit;
        }

        internal Transaction(TradingOrder? originalOrder, decimal executionPrice, DateTime executionTime)
        {
            ExecutionPrice = executionPrice;
            ExecutionTime = executionTime;
            OriginalOrder = originalOrder;
        }

        /// <summary>
        /// The type of transaction
        /// </summary>
        public TransactionType Type { get; set; }

        public FinancialInstrument? FinancialInstrument { get; set; }

        public TradingOrderType OrderType { get; set; }

        public int Quantity { get; set; }

        public decimal ExecutionPrice { get; set; }

        public DateTime ExecutionTime { get; set; }

        public TradingOrder? OriginalOrder { get; set; }

        public decimal CommissionPaid { get; set; }

        public bool IsStopLoss { get; }

        public bool IsTakeProfit { get; }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2} {3} @{4}", ExecutionTime.ToString(), OrderType.ToString(), Quantity.ToString(),
                FinancialInstrument?.Ticker, ExecutionPrice.ToString("0.00"));
        }
    }
}
