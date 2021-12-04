using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Models;

namespace VeniceDomain.Extensions
{
    public static class FinancialInstrumentExtension
    {
        public static decimal GetTotalDividends(this FinancialInstrument financialInstrument, IEnumerable<Transaction> transactions, BrokerEnvironment brokerEnvironment)
        {
            if (financialInstrument.Dividends == null || financialInstrument.Dividends.Count == 0)
            {
                return 0M;
            }

            decimal totalDividends = 0;
            int currentQuantity = 0;
            List<Transaction> transactionsToConsider = transactions.OrderBy(t => t.ExecutionTime).ToList();
            List<Dividend> dividendsToAdd = financialInstrument.Dividends
                .Where(d => d.Date > transactionsToConsider[0].ExecutionTime)
                .OrderBy(d => d.Date)
                .ToList();

            for (int i = 0; i < transactionsToConsider.Count; i++)
            {
                Transaction currentTransaction = transactionsToConsider[i];
                Transaction nextTransaction = i == transactionsToConsider.Count - 1 ? null : transactionsToConsider[i + 1];
                if (dividendsToAdd.Count == 0)
                {
                    break;
                }
                currentQuantity += currentTransaction.OrderType == Enums.TradingOrderType.BUY ? currentTransaction.Quantity : -currentTransaction.Quantity;
                if (currentQuantity <= 0)
                {
                    continue;
                }

                while (dividendsToAdd.Count > 0 && currentTransaction.ExecutionTime <= dividendsToAdd[0].Date)
                {
                    if (nextTransaction != null)
                    {
                        if (dividendsToAdd[0].Date > nextTransaction.ExecutionTime)
                        {
                            break;
                        }
                    }

                    totalDividends += dividendsToAdd[0].ValuePerUnit * currentQuantity * (1M - brokerEnvironment.NetEarningTaxation);
                    dividendsToAdd.RemoveAt(0);
                }
            }
            return totalDividends;
        }
    }
}
