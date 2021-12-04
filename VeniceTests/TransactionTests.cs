using System;
using System.Collections.Generic;
using VeniceDomain.Extensions;
using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class TransactionTests
    {
        [Fact]
        public void TestNetEarning()
        {
            var financialInstrument = new FinancialInstrument()
            {
                CommercialName = "tango",
                Isin = "agfadg"
            };
            var broker = new BrokerEnvironment()
            {
                AnnualTax = 0.02M,
                CommissionPlan = new PercentageMinMaxCommissionPlan()
                {
                    MinCommission = 1.5M,
                    MaxCommission = 19,
                    CommissionPercentage = 0.0019M
                },
                NetEarningTaxation = 0.26M
            };
            // First order: buying 40 units @ 15€ 01/01/2016 //
            List<Transaction> transactions = new()
            {
                new Transaction(financialInstrument, 15M, new DateTime(2016, 1, 1), null, VeniceDomain.Enums.TradingOrderType.BUY, 40)
            };
            // Candle for the 15/01/16: 13€ close //
            var candle = new CandleValue() { Close = 13M, FinancialInstrument = financialInstrument, StartDate = new DateTime(2016, 1, 15) };
            // First check //
            var balance = transactions.GetNetEarning(candle, broker);
            Assert.Equal(-83, balance);

            // Second order: buying 20 units @ 14€ 02/02/16 //
            transactions.Add(new Transaction(financialInstrument, 14M, new DateTime(2016, 2, 2), null, VeniceDomain.Enums.TradingOrderType.BUY, 20));
            // Candle for the 15/2/16: 15,5€ close //
            var candleTwo = new CandleValue() { Close = 15.5M, FinancialInstrument = financialInstrument, StartDate = new DateTime(2016, 2, 15) };
            var balanceTwo = transactions.GetNetEarning(candleTwo, broker);
            Assert.Equal(32.233M, balanceTwo, 2);

            // Third order: selling 15 units @ 17€ 03/07/2016
            transactions.Add(new Transaction(financialInstrument, 17M, new DateTime(2016, 7, 3), null, VeniceDomain.Enums.TradingOrderType.SELL, 15));
            // Candle for the 01/08/16: 18,70€ close //
            var candleThree = new CandleValue() { Close = 18.7M, FinancialInstrument = financialInstrument, StartDate = new DateTime(2016, 8, 1) };
            var balanceThree = transactions.GetNetEarning(candleThree, broker);
            Assert.Equal(154.11M, balanceThree, 2);

            // Fourth order: selling all remainings (45) @20€ //
            transactions.Add(new Transaction(financialInstrument, 20M, new DateTime(2016, 7, 3), null, VeniceDomain.Enums.TradingOrderType.SELL, 45));
            // Candle for the 01/10/16: 21,00€ close //
            var candleFour = new CandleValue() { Close = 21M, FinancialInstrument = financialInstrument, StartDate = new DateTime(2016, 10, 1) };
            var balanceFour = transactions.GetNetEarning(candleFour, broker);
            Assert.Equal(197.29M, balanceFour, 2);

            // Candle for the 20/10/16: 22,00€ close //
            // Nothing should change, since everything was sold //
            var candleFive = new CandleValue() { Close = 22M, FinancialInstrument = financialInstrument, StartDate = new DateTime(2016, 10, 20) };
            var balanceFive = transactions.GetNetEarning(candleFive, broker);
            Assert.Equal(197.29M, balanceFive, 2);
        }
    }
}
