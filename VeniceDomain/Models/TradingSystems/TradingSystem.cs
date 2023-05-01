using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Enums;
using VeniceDomain.Models.Base;
using VeniceDomain.Models.TradingSystems;
using VeniceDomain.Services;

namespace VeniceDomain.Models
{
    public abstract class TradingSystem : BaseTradingAction
    {
        #region Constructor

        public TradingSystem() { }

        public TradingSystem(FinancialInstrument financialInstrument)
            : this()
        {
            ArgsValidationUtility.EnsureNotNull(financialInstrument);
            FinancialInstrument = financialInstrument;
        }

        public TradingSystem(FinancialInstrument financialInstrument, List<CandleValue> candles)
            : this(financialInstrument)
        {
            Candles = candles?.OrderBy(c => c.StartDate).ToList() ?? new List<CandleValue>();
        }

        public TradingSystem(FinancialInstrument financialInstrument, List<CandleValue> candles, List<TradingSystemParameter> parameters)
            : this(financialInstrument, candles)
        {
            Parameters = parameters;
            SubscribeToParameterValueChangedCallback();
        }

        #endregion

        #region Instance properties

        /// <summary>
        /// An identifier for the user
        /// </summary>
        public string Name { get; set; }

        public event Action<List<TradingSystemOrder>> OnShouldExecuteOrder;

        public TradingSystemState State { get; private set; } = TradingSystemState.IDLE;

        public virtual List<CandleValue> Candles { get; set; } = new List<CandleValue>();

        public virtual List<TradingSystemParameter> Parameters { get; protected set; } = new List<TradingSystemParameter>();

        public int? ConfigurationId { get; set; }

        public List<TradingSystemOrder> OrdersSent { get; } = new List<TradingSystemOrder>();

        protected static readonly TradingSystemParameter UNINITIALIZED_PARAMETER = new TradingSystemParameter("", 0);

        #endregion

        protected abstract IEnumerable<TradingSystemOrder> ShouldExecuteNext();

        protected virtual TradingOrderType[] GetOrderTypesToSend(TradingOrderType orderType)
        {
            TradingOrderType[] ordersToSend = new TradingOrderType[0];
            if (State == TradingSystemState.IDLE)
            {
                if (orderType == TradingOrderType.BUY || orderType == TradingOrderType.SHORT_SELL)
                {
                    ordersToSend = new[] { orderType };
                }
            }
            else
            {
                switch (orderType)
                {
                    case TradingOrderType.BUY:
                        if (State == TradingSystemState.SHORT)
                            ordersToSend = new[] { TradingOrderType.SHORT_BUY, TradingOrderType.BUY };
                        break;

                    case TradingOrderType.SELL:
                        if (State == TradingSystemState.LONG)
                            ordersToSend = new[] { TradingOrderType.SELL };
                        break;

                    case TradingOrderType.SHORT_SELL:
                        if (State == TradingSystemState.LONG)
                            ordersToSend = new[] { TradingOrderType.SELL, TradingOrderType.SHORT_SELL };
                        break;

                    case TradingOrderType.SHORT_BUY:
                        if (State == TradingSystemState.SHORT)
                            ordersToSend = new[] { TradingOrderType.SHORT_BUY };
                        break;
                }
            }
            
            State = orderType.GetTradingSystemState(State);
            return ordersToSend;
        }

        protected IEnumerable<TradingSystemOrder> ConvertOrderTypesToOrders(TradingOrderType[] ordersToSend)
        {
            foreach (TradingOrderType type in ordersToSend)
            {
                yield return new TradingSystemOrder(FinancialInstrument, type, Candles[^1], Candles.Count - 1);
            }
        }

        public TradingSystemParameter GetParameterByName(string name) => Parameters.Single(p => p.Name == name);

        public virtual void AddCandle(CandleValue candleValue)
        {
            Candles.Add(candleValue);
            // Fires the order //
            List<TradingSystemOrder> orders = ShouldExecuteNext().ToList();
            foreach (TradingSystemOrder order in orders)
            {
                OrdersSent.Add(order);
                Log("Trading system is creating an order: " + order.ToString());
            }
            OnShouldExecuteOrder?.Invoke(orders);
        }

        /// <summary>
        /// Returns all possible orders created with given <paramref name="candleValues"/> list<br/>
        /// Usually used to test the trading system
        /// </summary>
        public virtual IEnumerable<TradingSystemOrder> GetAllOrders(IEnumerable<CandleValue> candleValues, bool shouldCalculateIndicators)
        {
            foreach (CandleValue candle in candleValues)
            {
                Candles.Add(candle);
                foreach (TradingSystemOrder order in ShouldExecuteNext())
                {
                    OrdersSent.Add(order);
                    yield return order;
                }
            }
        }

        /// <summary>
        /// Resets this trading system historical data, to let it be used again with new candles
        /// </summary>
        public virtual void ResetStatus()
        {
            Candles = new List<CandleValue>();
            State = TradingSystemState.IDLE;
        }

        protected virtual void OnParameterCurrentValueChanged(TradingSystemParameter parameter, decimal currentValue)
        {
        }

        private void SubscribeToParameterValueChangedCallback()
        {
            foreach (TradingSystemParameter parameter in Parameters)
            {
                if (parameter != null)
                {
                    parameter.OnCurrentValueChanged += delegate (decimal value)
                    {
                        OnParameterCurrentValueChanged(parameter, value);
                    };
                }
            }
        }

        private static void Log(string message, LogImportance importance = LogImportance.Debug)
            => VeniceLogger.Log(message, nameof(TradingSystem), importance);
    }
}
