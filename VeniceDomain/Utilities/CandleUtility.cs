using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Models;

namespace VeniceDomain.Utilities
{
    internal static class CandleUtility
    {
        internal static List<TradingOperation> GetTradingOperations(BrokerEnvironment brokerEnvironment, List<Transaction> transactions)
            => GetOperationsEnumerable(brokerEnvironment, transactions).ToList();

        private static IEnumerable<TradingOperation> GetOperationsEnumerable(BrokerEnvironment brokerEnvironment, List<Transaction> transactions)
        {
            for(int i = 0; i < transactions.Count - 1; i += 2)
                // Cycling all transactions, but not the last one //
            {
                Transaction openingTransaction = transactions[i];
                Transaction closingTransaction = null;
                if (i != transactions.Count - 1)
                    closingTransaction = transactions[i + 1];

                yield return new TradingOperation(brokerEnvironment, openingTransaction, closingTransaction);
            }
        }
    }
}
