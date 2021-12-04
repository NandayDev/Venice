using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeniceDomain.Enums;
using VeniceDomain.Models;

namespace VeniceDomain.Interfaces
{
    /// <summary>
    /// An interface for a trading provider<br/>
    /// The provider is responsible for giving candle values to the client, letting order execute and check the status of the portfolio
    /// </summary>
    public interface ITradingProvider
    {
        /// <summary>
        /// Commission plan for this provider
        /// </summary>
        CommissionPlan StockCommissionPlan { get; }

        /// <summary>
        /// Returns an IEnumerable of <see cref="CandleResponse"/> from given request
        /// </summary>
        Task<IEnumerable<CandleResponse>> GetCandle(CandleRequest candleRequest);

        /// <summary>
        /// Executes given order
        /// </summary>
        Task<Transaction> ExecuteOrder(TradingOrder tradingOrder);

        /// <summary>
        /// Gets current available liquidity
        /// </summary>
        Task<decimal> GetAvailableLiquidity();

        /// <summary>
        /// Returns TRUE if the connection is available and running
        /// </summary>
        Task<bool> IsConnectionAvailable();

        /// <summary>
        /// Returns current quantity in portfolio for given <see cref="FinancialInstrument"/>
        /// </summary>
        Task<int> GetFinancialInstrumentPortfolioQuantity(FinancialInstrument instrument);

        /// <summary>
        /// Subscribes to the price changed feed for given <paramref name="instrument"/>.<br/>
        /// The <paramref name="onDataFeedReceived"/> action passed will be performed every time a new price for given <paramref name="instrument"/> comes out
        /// </summary>
        Task SubscribeToPriceFeed(FinancialInstrument instrument, CandlePeriod period, Action<CandleValue> onDataFeedReceived);

        /// <summary>
        /// Unsubscribes to the price changed feed
        /// </summary>
        Task UnsubscribeToPriceFeed(FinancialInstrument instrument, Action<CandleValue> onDataFeedReceived);

        /// <summary>
        /// Returns the default starting date for candle requests with given <paramref name="period"/>
        /// </summary>
        DateTime GetDefaultStartingDateForRequests(CandlePeriod period);


        #region Default interface implementation

        /// <summary>
        /// Fills the <paramref name="currentCandles"/> list with the missing candles by querying the <see cref="ITradingProvider"/>
        /// </summary>
        public async Task<List<CandleResponse>> GetMissingCandles(List<CandleValue> currentCandles, FinancialInstrument financialInstrument, CandlePeriod period)
        {
            List<CandleResponse> candleResponses = new List<CandleResponse>();
            DateTime lastCandleDate = GetDefaultStartingDateForRequests(period);
            if (currentCandles != null && currentCandles.Any())
            {
                lastCandleDate = currentCandles.OrderBy(c => c.StartDate).Last().EndDate;
            }
            CandleRequest candleRequest = new CandleRequest(financialInstrument, period, lastCandleDate, DateTime.Now);
            var responses = await GetCandle(candleRequest);
            foreach (var candleFromProvider in responses)
            {
                if (!currentCandles.Any(c => c.Equals(candleFromProvider.CandleValue)))
                    candleResponses.Add(candleFromProvider);
            }
            return candleResponses;
        }

        #endregion
    }
}
