using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Models;

namespace VeniceDomain.Extensions
{
    public static class TradingResponseExtension
    {
        public static IEnumerable<TradingQueryResult> GetNetEarnings(this IEnumerable<Transaction> transactions,
            List<CandleValue> candles, BrokerEnvironment brokerEnviroment)
        {
            var dateGroups = candles.GroupBy(c => c.EndDate.Date);
            foreach (var group in dateGroups)
            {
                var queryResult = new TradingQueryResult(group.Key);
                if (group.Count() == 0)
                    continue;
                foreach (var candle in group)
                {
                    decimal netEarning = transactions.GetNetEarning(candle, brokerEnviroment);
                    queryResult.NetEarnings.Add(candle.FinancialInstrument, netEarning);
                    decimal counterValue = 0;
                    FinancialInstrument financialInstrument = candle.FinancialInstrument;
                    var transactionsToCheck = transactions
                        .Where(t => t.ExecutionTime.Date <= candle.EndDate.Date)
                        .Where(t => t.FinancialInstrument.Equals(candle.FinancialInstrument));
                    decimal weightedAvgPrice = transactionsToCheck.GetWeightedAverageBuyingPrice(financialInstrument);
                    counterValue = weightedAvgPrice * transactionsToCheck.GetNetQuantity();
                    queryResult.CounterValues.Add(financialInstrument, counterValue);
                    if (counterValue != 0)
                    {
                        queryResult.NetPercentEarnings.Add(financialInstrument, (netEarning / counterValue) * 100);
                    }
                }
                yield return queryResult;
            }
        }

        public static decimal GetNetEarning(this IEnumerable<Transaction> transactions, CandleValue mostRecentCandle,
            BrokerEnvironment brokerEnviroment)
        {
            // First filter to be sure //
            var transactionsToCheck = transactions
                .Where(t => t.ExecutionTime.Date <= mostRecentCandle.EndDate.Date)
                .Where(t => t.FinancialInstrument.Equals(mostRecentCandle.FinancialInstrument));
            if (transactionsToCheck.Count() == 0)
                return 0;
            decimal markToMarketValue = mostRecentCandle.GetMarkToMarketValue(transactionsToCheck);
            decimal weightedAvgPrice = transactionsToCheck.GetWeightedAverageBuyingPrice(mostRecentCandle.FinancialInstrument);
            decimal counterValue = weightedAvgPrice * transactionsToCheck.GetNetQuantity();
            var grossDifference = markToMarketValue - counterValue;
            if (grossDifference > 0)
                grossDifference = grossDifference * (1M - brokerEnviroment.NetEarningTaxation);
            var commissions = transactionsToCheck.GetCommissions(mostRecentCandle, brokerEnviroment);
            decimal dividends = mostRecentCandle.FinancialInstrument.GetTotalDividends(transactionsToCheck, brokerEnviroment);
            decimal sellBalance = transactionsToCheck.GetSellBalance(weightedAvgPrice, brokerEnviroment);
            var netEarning = grossDifference - commissions.Sum() + dividends + sellBalance;
            return netEarning;
        }

        public static decimal GetWeightedAverageBuyingPrice(this IEnumerable<Transaction> tradingResponses, FinancialInstrument financialInstrument)
        {
            decimal numerator = 0;
            decimal denominator = 0;
            foreach(var response in tradingResponses.Where(t => t.FinancialInstrument.Equals(financialInstrument) && t.OrderType == TradingOrderType.BUY))
            {
                numerator += response.ExecutionPrice * response.Quantity;
                denominator += response.Quantity;
            }
            return denominator == 0 ? 0 : numerator / denominator;
        }

        public static IEnumerable<decimal> GetCommissions(this IEnumerable<Transaction> transactions, CandleValue mostRecentCandle,
            BrokerEnvironment brokerEnviroment)
        {
            foreach (Transaction transaction in transactions)
            {
                if (transaction.CommissionPaid == 0)
                    transaction.CommissionPaid = brokerEnviroment.CommissionPlan.GetCommissionOnOperation(transaction);
                yield return transaction.CommissionPaid;
            }
            var imaginaryTradingResponse = new Transaction()
            {
                ExecutionPrice = mostRecentCandle.Close,
                Quantity = transactions.GetNetQuantity()
            };
            if (imaginaryTradingResponse.Quantity > 0)
                yield return brokerEnviroment.CommissionPlan.GetCommissionOnOperation(imaginaryTradingResponse);
        }

        public static int GetNetQuantity(this IEnumerable<Transaction> transactions)
        {
            return transactions.Where(t => t.OrderType == TradingOrderType.BUY).Select(t => t.Quantity).Sum()
                - transactions.Where(t => t.OrderType == TradingOrderType.SELL).Select(t => t.Quantity)?.Sum() ?? 0;
        }

        public static decimal GetSellBalance(this IEnumerable<Transaction> tradingResponses, decimal weightedAvgPrice,
            BrokerEnvironment brokerEnviroment)
        {
            decimal balance = 0;
            var sellResponses = tradingResponses.Where(t => t.OrderType == TradingOrderType.SELL);
            foreach (var sell in sellResponses)
            {
                var gross = (sell.ExecutionPrice - weightedAvgPrice) * sell.Quantity;
                if (gross < 0)
                    balance += gross;
                else
                    balance += gross * (1 - brokerEnviroment.NetEarningTaxation);
            }
            return balance;
        }

    }
}
