using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeniceDomain.Enums;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class DividendTests
    {
        private readonly BrokerEnvironment brokerEnvironment = new()
        {
            NetEarningTaxation = 0.26M,
            CommissionPlan = new PercentageMinMaxCommissionPlan()
            {
                CommissionPercentage = 0.0019M,
                MinCommission = 1.5M,
                MaxCommission = 18M,
            }
        };

        private FinancialInstrument financialInstrument;
        private List<Transaction> transactions;

        [Fact]
        public void TestDividendsOneAfterAnother()
        {
            financialInstrument = new();
            transactions = new();

            // One transaction, no dividends //
            AddTransaction(2017, 10, 1, 12, 2.4M, TradingOrderType.BUY);
            decimal dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(0M, dividends);

            // Adding a dividend BEFORE the transaction //
            AddDividend(2017, 3, 2, 0.4M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(0M, dividends);

            // Adding a dividend AFTER the transaction //
            AddDividend(2018, 3, 3, 0.5M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M, dividends);

            // Adding another transaction after the dividend //
            AddTransaction(2018, 4, 2, 15, 2.3M, TradingOrderType.BUY);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M, dividends);

            // Now a dividend after the transaction... and so on //
            AddDividend(2018, 5, 6, 0.45M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M + 8.991M, dividends);

            // Now let's try selling: first a part... //
            AddTransaction(2019, 6, 6, 10, 3M, TradingOrderType.SELL);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M + 8.991M, dividends);

            AddDividend(2020, 7, 7, 0.3M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M + 8.991M + 3.774M, dividends);

            // Now selling all that remains //
            AddTransaction(2020, 9, 10, 17, 3.4M, TradingOrderType.SELL);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M + 8.991M + 3.774M, dividends);

            // Since there are no stocks remaining, this should return the same amount as before //
            AddDividend(2020, 11, 15, 0.6M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(4.44M + 8.991M + 3.774M, dividends);
        }

        [Fact]
        public void TestDividendsTogether()
        {
            financialInstrument = new();
            transactions = new();

            // One transaction, no dividends //
            AddTransaction(2017, 10, 1, 12, 2.4M, TradingOrderType.BUY);
            decimal dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(0M, dividends);

            AddDividend(2018, 3, 2, 0.4M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(3.552M, dividends);

            AddDividend(2018, 5, 6, 0.45M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(3.552M + 3.996M, dividends);

            AddTransaction(2019, 10, 1, 12, 2.4M, TradingOrderType.SELL);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(3.552M + 3.996M, dividends);

            AddDividend(2020, 5, 6, 0.45M);
            dividends = financialInstrument.GetTotalDividends(transactions, brokerEnvironment);
            Assert.Equal(3.552M + 3.996M, dividends);
        }

        private void AddTransaction(int year, int month, int day, int quantity, decimal price, TradingOrderType orderType)
        {
            transactions.Add(new Transaction
            {
                OrderType = orderType,
                ExecutionTime = new DateTime(year, month, day),
                Quantity = quantity,
                ExecutionPrice = price
            });
        }

        private void AddDividend(int year, int month, int day, decimal amountPerUnit)
        {
            financialInstrument.Dividends.Add(new Dividend
            {
                Date = new DateTime(year, month, day),
                FinancialInstrument = financialInstrument,
                ValuePerUnit = amountPerUnit
            });
        }
    }
}
