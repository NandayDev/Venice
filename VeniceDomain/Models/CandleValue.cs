using System;
using System.Collections.Generic;
using VeniceDomain.Enums;
using VeniceDomain.Models.Base;
using VeniceDomain.Models.TechnicalAnalysis;
using VeniceDomain.Utilities;

namespace VeniceDomain.Models
{
    public class CandleValue : BaseTradingAction
    {
        #region Constructors

        public CandleValue()
        {
        }

        public CandleValue(FinancialInstrument instrument, DateTime startDate, CandlePeriod candlePeriod, decimal low, decimal high, decimal open, decimal close)
        {
            FinancialInstrument = instrument;
            StartDate = startDate;
            CandlePeriod = candlePeriod;
            Low = low;
            High = high;
            Open = open;
            Close = close;
        }

        public CandleValue(FinancialInstrument instrument, DateTime startDate, CandlePeriod candlePeriod, decimal low, decimal high, decimal open, decimal close, int volume)
            : this(instrument, startDate, candlePeriod, low, high, open, close)
        {
            Volume = volume;
        }

        #endregion

        #region Instance properties

        public DateTime StartDate { get; set; }

        public CandlePeriod CandlePeriod { get; set; }

        public decimal Low { get; set; }

        public decimal High { get; set; }

        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public int Volume { get; set; }

        public DateTime EndDate => TimeUtility.AddPeriodToDate(StartDate, CandlePeriod); // can be performance tweaked, saving the enddate and nulling it when candleperiod is changed

        private readonly Dictionary<string, BaseTechnicalAnalysisIndicator> indicators = new Dictionary<string, BaseTechnicalAnalysisIndicator>();

        #endregion

        #region Public methods

        /// <summary>
        /// Adds or overwrites given <paramref name="indicator"/> for given <paramref name="key"/>
        /// </summary>
        internal void AddIndicator(string key, BaseTechnicalAnalysisIndicator indicator)
        {
            indicators[key] = indicator;
        }

        /// <summary>
        /// Returns indicator of given type <typeparamref name="TIndicator"/> or null if not present
        /// </summary>
        internal TIndicator? GetIndicator<TIndicator>(string key)
            where TIndicator : BaseTechnicalAnalysisIndicator
        {
            if (false == indicators.ContainsKey(key))
                return null;
            return (TIndicator)indicators[key];
        }

        #endregion

        #region Overridden methods

        public override string ToString()
        {
            return string.Format("{0} - {1} - Close {2} - Open {3} - High {4} - Low {5}",
                FinancialInstrument?.Ticker ?? "",
                StartDate.ToString(),
                Close,
                Open,
                High,
                Low);
        }

        /// <summary>
        /// Compares two candles
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is CandleValue candle)
            {
                return 
                    candle.StartDate == StartDate 
                    && candle.CandlePeriod == CandlePeriod
                    && candle.Low == Low 
                    && candle.High == High 
                    && candle.Open == Open 
                    && candle.Close == Close
                    && candle.Volume == Volume;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -592064986;
            hashCode = hashCode * -1521134295 + StartDate.GetHashCode();
            hashCode = hashCode * -1521134295 + CandlePeriod.GetHashCode();
            hashCode = hashCode * -1521134295 + Low.GetHashCode();
            hashCode = hashCode * -1521134295 + High.GetHashCode();
            hashCode = hashCode * -1521134295 + Open.GetHashCode();
            hashCode = hashCode * -1521134295 + Close.GetHashCode();
            return hashCode;
        }

        #endregion
    }
}
