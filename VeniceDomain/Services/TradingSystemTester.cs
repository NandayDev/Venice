using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Models;
using VeniceDomain.Models.TradingSystems;
using VeniceDomain.Utilities;

namespace VeniceDomain.Services
{
    public class TradingSystemTester
    {
        #region Constructor

        public TradingSystemTester(
            TradingSystem tradingSystem,
            decimal investedAmount,
            BrokerEnvironment brokerEnvironment,
            bool closeLastOperation)
        {
            this.tradingSystem = tradingSystem;
            currentAvailableFunds = investedAmount;
            BrokerEnvironment = brokerEnvironment;
            shouldCloseLastOperation = closeLastOperation;
        }

        #endregion

        #region Instance properties

        public BrokerEnvironment BrokerEnvironment { get; set; }

        private readonly TradingSystem tradingSystem;

        private readonly decimal currentAvailableFunds;

        private readonly bool shouldCloseLastOperation;

        public event Action<int> OnReportsProgressUpdated;

        #endregion

        /// <summary>
        /// Generates a single <see cref="TradingSystemReport"/> with given <see cref="TradingSystem"/>, based on the current TradingSystemParameters
        /// </summary>
        public TradingSystemReport GetSingleReport(List<CandleValue> candleValues, bool shouldCalculateIndicators, out List<TradingOperation> operations)
        {
            var orders = tradingSystem.GetAllOrders(candleValues, true).ToList();
            var transactions = new List<Transaction>();
            for (int i = 0; i < orders.Count; i++)
            {
                TradingSystemOrder order = orders[i];
                // The order becomes effective on the next candle, since it's impossible to buy on the close, //
                // which determines the trading system status //
                int candleValueIndex = candleValues.IndexOf(order.CandleThatMeetsConditions);
                CandleValue nextCandleAfterOrder = candleValues.ElementAtOrDefault(candleValueIndex + 1);
                if (nextCandleAfterOrder != null)
                {
                    transactions.Add(
                        new Transaction(
                            order.FinancialInstrument,
                            nextCandleAfterOrder.Open,
                            nextCandleAfterOrder.StartDate,
                            null,
                            order.OrderType, 
                            0
                        )
                    );

                    if (order.FlatAtNextClose)
                    {
                        // Order is closed at this candle's close //
                        decimal transactionClosePrice = nextCandleAfterOrder.Close;
                        if (order.StopLossPrice != null)
                        {
                            switch (order.OrderType)
                            {
                                case TradingOrderType.BUY:
                                    decimal stopLossPrice = nextCandleAfterOrder.Open * (1 - (order.StopLossPrice.Value / 100m));
                                    if (nextCandleAfterOrder.Low <= stopLossPrice)
                                    {
                                        transactionClosePrice = stopLossPrice;
                                    }
                                    break;

                                case TradingOrderType.SHORT_SELL:
                                    stopLossPrice = nextCandleAfterOrder.Open * (1 + (order.StopLossPrice.Value / 100m));
                                    if (nextCandleAfterOrder.High >= stopLossPrice)
                                    {
                                        transactionClosePrice = stopLossPrice;
                                    }
                                    break;

                                default:
                                    throw new ArgumentException("Order type can't be other than buy or short sell");
                            }
                        }
                        if (order.TakeProfitPrice != null)
                        {
                            switch(order.OrderType)
                            {
                                case TradingOrderType.BUY:
                                    decimal takeProfitPrice = nextCandleAfterOrder.Open * (1 + (order.TakeProfitPrice.Value / 100m));
                                    if (nextCandleAfterOrder.High > takeProfitPrice)
                                    {
                                        transactionClosePrice = takeProfitPrice;
                                    }
                                    break;

                                case TradingOrderType.SHORT_SELL:
                                    takeProfitPrice = nextCandleAfterOrder.Open * (1 - (order.TakeProfitPrice.Value / 100m));
                                    if (nextCandleAfterOrder.Low < takeProfitPrice)
                                    {
                                        transactionClosePrice = takeProfitPrice;
                                    }
                                    break;

                                default:
                                    throw new ArgumentException("Order type can't be other than buy or short sell");
                            }
                        }
                        transactions.Add(
                            new Transaction(
                                order.FinancialInstrument,
                                transactionClosePrice,
                                nextCandleAfterOrder.EndDate,
                                null,
                                order.OrderType == TradingOrderType.BUY ? TradingOrderType.SELL : TradingOrderType.SHORT_BUY,
                                0));
                    }
                }
            }
            operations = CandleUtility.GetTradingOperations(BrokerEnvironment, transactions);
            // Let's see if there's an open operation to add //
            if (shouldCloseLastOperation && transactions.Count % 2 != 0)
            {
                operations.Add(
                    new TradingOperation(
                        BrokerEnvironment,
                        transactions[^1],
                        new Transaction(
                            orders[0].FinancialInstrument,
                            candleValues[^1].Close,
                            candleValues[^1].EndDate,
                            null,
                            transactions[^1].OrderType == TradingOrderType.BUY ? TradingOrderType.SELL : TradingOrderType.SHORT_BUY,
                            0)
                        )
                    );
            }
            return new TradingSystemReport(
                operations, 
                BrokerEnvironment,
                tradingSystem.Parameters,
                currentAvailableFunds
            );
        }

        /// <summary>
        /// Generates all possible reports with given <see cref="TradingSystem"/> and all values, from min to max, of the TradingSystemParameters
        /// </summary>
        public List<TradingSystemReport> GetReports(List<CandleValue> candleValues, bool shouldCalculateIndicators)
        {
            List<TradingSystemReport> reportList = new List<TradingSystemReport>();

            ResetParametersToMinimumValue();
            int lastProgressSent = 0;

            List<List<decimal>> a = new List<List<decimal>>();
            foreach(var parameter in tradingSystem.Parameters)
            {
                var l = new List<decimal>();
                a.Add(l);
                for (parameter.CurrentValue = parameter.MinValue;
                    parameter.CurrentValue <= parameter.MaxValue;
                    parameter.CurrentValue += parameter.Step)
                {
                    l.Add(parameter.CurrentValue);
                }
            }
            var tradingSystemCombinations = new List<List<TradingSystemParameter>>();
            GetAllTradingParameterCombinations(a, tradingSystemCombinations, new List<TradingSystemParameter>(), tradingSystem.Parameters);

            for (int i = 0; i < tradingSystemCombinations.Count; i++)
            {
                var parameterList = tradingSystemCombinations[i];
                for (int k = 0; k < parameterList.Count; k++)
                {
                    tradingSystem.Parameters[k].CurrentValue = parameterList[k].CurrentValue;
                }
                tradingSystem.ResetStatus();
                reportList.Add(GetSingleReport(candleValues, shouldCalculateIndicators, out _));
                int progress = (int)(i / (double)tradingSystemCombinations.Count * 100);
                if (progress != lastProgressSent)
                {
                    OnReportsProgressUpdated?.Invoke(progress);
                    lastProgressSent = progress;
                }
            }

            return reportList;
        }

        void GetAllTradingParameterCombinations(List<List<decimal>> domains, List<List<TradingSystemParameter>> outcome, List<TradingSystemParameter> vector, List<TradingSystemParameter> originalParameters)
        {
            if (domains.Count == vector.Count)
            {
                outcome.Add(vector);
                return;
            }
            foreach (var value in domains[vector.Count])
            {
                var newVector = vector.ToList();
                newVector.Add(new TradingSystemParameter(originalParameters[vector.Count].Name, value));
                GetAllTradingParameterCombinations(domains, outcome, newVector, originalParameters);
            }
        }

        private void ResetParametersToMinimumValue(TradingSystemParameter parameterToLeaveUntouched = null)
        {
            foreach (var parameter in tradingSystem.Parameters)
            {
                if (parameter == parameterToLeaveUntouched)
                    continue;
                parameter.CurrentValue = parameter.MinValue;
            }
        }
    }
}
 