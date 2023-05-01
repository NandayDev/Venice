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
        public TradingSystemReport GetSingleReport(List<CandleValue> candleValues, out List<TradingOperation> operations)
        {
            var orders = tradingSystem.GetAllOrders(candleValues, true).ToList();
            var transactions = new List<Transaction>();
            for (int i = 0; i < orders.Count; i++)
            {
                TradingSystemOrder order = orders[i];
                // The order becomes effective on the next candle, since it's impossible to buy on the close, //
                // which determines the trading system status //
                CandleValue? nextCandle = null;
                if (order.CandleThatMeetsConditionIndex < tradingSystem.Candles.Count - 1)
                {
                    nextCandle = tradingSystem.Candles[order.CandleThatMeetsConditionIndex + 1];
                }
                if (nextCandle != null)
                {
                    transactions.Add(
                        new Transaction(
                            order.FinancialInstrument,
                            nextCandle.Open,
                            nextCandle.StartDate,
                            null,
                            order.OrderType, 
                            0
                        )
                    );

                    if (order.FlatAtNextClose)
                    {
                        // Order is closed at this candle's close //
                        decimal transactionClosePrice = nextCandle.Close;
                        bool isStopLoss = false;
                        bool isTakeProfit = false;
                        if (order.StopLossPercentage != null)
                        {
                            switch (order.OrderType)
                            {
                                case TradingOrderType.BUY:
                                    decimal stopLossPrice = nextCandle.Open * (1 - (order.StopLossPercentage.Value / 100m));
                                    if (nextCandle.Low <= stopLossPrice)
                                    {
                                        transactionClosePrice = stopLossPrice;
                                        isStopLoss = true;
                                    }
                                    break;

                                case TradingOrderType.SHORT_SELL:
                                    stopLossPrice = nextCandle.Open * (1 + (order.StopLossPercentage.Value / 100m));
                                    if (nextCandle.High >= stopLossPrice)
                                    {
                                        transactionClosePrice = stopLossPrice;
                                        isStopLoss = true;
                                    }
                                    break;

                                default:
                                    throw new ArgumentException("Order type can't be other than buy or short sell");
                            }
                        }
                        else if (order.TakeProfitPercentage != null)
                        {
                            switch(order.OrderType)
                            {
                                case TradingOrderType.BUY:
                                    decimal takeProfitPrice = nextCandle.Open * (1 + (order.TakeProfitPercentage.Value / 100m));
                                    if (nextCandle.High > takeProfitPrice)
                                    {
                                        transactionClosePrice = takeProfitPrice;
                                        isTakeProfit = true;
                                    }
                                    break;

                                case TradingOrderType.SHORT_SELL:
                                    takeProfitPrice = nextCandle.Open * (1 - (order.TakeProfitPercentage.Value / 100m));
                                    if (nextCandle.Low < takeProfitPrice)
                                    {
                                        transactionClosePrice = takeProfitPrice;
                                        isTakeProfit = true;
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
                                nextCandle.EndDate,
                                null,
                                order.OrderType == TradingOrderType.BUY ? TradingOrderType.SELL : TradingOrderType.SHORT_BUY,
                                0, 
                                isStopLoss, 
                                isTakeProfit)
                            );
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
        public List<TradingSystemReport> GetReports(List<CandleValue> candleValues)
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
                    if (parameter.Step == 0)
                    {
                        break;
                    }
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
                reportList.Add(GetSingleReport(candleValues, out _));
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
 