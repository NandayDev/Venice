using VeniceDomain.Models;

namespace VeniceDomain.Extensions
{
    public static class ITradingEventExtension
    {
        public static decimal GetTransactionTotalValue(this Transaction transaction) => transaction.ExecutionPrice * transaction.Quantity;
    }
}
