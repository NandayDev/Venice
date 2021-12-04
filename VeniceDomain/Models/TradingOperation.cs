using System;
using VeniceDomain.Extensions;

namespace VeniceDomain.Models
{
    public class TradingOperation
    {
        public TradingOperation(BrokerEnvironment brokerEnvironment, Transaction openingTransaction, Transaction closingTransaction)
        {
            BrokerEnvironment = brokerEnvironment;
            OpeningTransaction = openingTransaction;
            ClosingTransaction = closingTransaction;
        }

        public BrokerEnvironment BrokerEnvironment { get; }

        public virtual Transaction OpeningTransaction { get; }

        public virtual Transaction ClosingTransaction { get; }

        private decimal? _netResult;
        public decimal NetResult => _netResult ??= GetNetResult();

        private decimal GetNetResult()
        {
            decimal grossResult = GetGrossResult();
            decimal firstCommission = BrokerEnvironment.CommissionPlan.GetCommissionOnOperation(OpeningTransaction);
            OpeningTransaction.CommissionPaid = firstCommission;
            decimal secondCommission = BrokerEnvironment.CommissionPlan.GetCommissionOnOperation(ClosingTransaction);
            ClosingTransaction.CommissionPaid = secondCommission;
            decimal annualTax = GetAnnualTax(BrokerEnvironment);
            if (grossResult < 0)
            {
                // Gross result negative: no application of net earning taxation //
                return grossResult - firstCommission - secondCommission - annualTax;
            }
            else
            {
                return Math.Round(grossResult * (1m - BrokerEnvironment.NetEarningTaxation), 2) - firstCommission - secondCommission - annualTax;
            }
        }

        private decimal GetGrossResult()
        {
            decimal firstTotalValue = OpeningTransaction.GetTransactionTotalValue();
            decimal secondTotalValue = ClosingTransaction.GetTransactionTotalValue();
            if (OpeningTransaction.OrderType == Enums.TradingOrderType.BUY)
            {
                return Math.Round(secondTotalValue - firstTotalValue, 2);
            }
            else if (OpeningTransaction.OrderType == Enums.TradingOrderType.SHORT_SELL)
            {
                return Math.Round(firstTotalValue - secondTotalValue, 2);
            }
            throw new Exception();
        }

        private decimal GetAnnualTax(BrokerEnvironment brokerEnviroment)
        {
            int yearDifference = (int)(ClosingTransaction.ExecutionTime - OpeningTransaction.ExecutionTime).TotalDays / 365;
            return OpeningTransaction.GetTransactionTotalValue() * brokerEnviroment.AnnualTax * yearDifference;
        }
    }
}
