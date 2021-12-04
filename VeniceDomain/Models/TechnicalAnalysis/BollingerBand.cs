namespace VeniceDomain.Models.TechnicalAnalysis
{
    public class BollingerBand : BaseTechnicalAnalysisIndicator
    {
        public BollingerBand() : base()
        {
        }

        public BollingerBand(CandleValue candleValue, int period, decimal standardDeviationMultiplier, decimal upperBandValue, decimal middleAverageValue, decimal lowerBandValue)
            : base(candleValue)
        {
            Period = period;
            StandardDeviationMultiplier = standardDeviationMultiplier;
            UpperBandValue = upperBandValue;
            MiddleAverageValue = middleAverageValue;
            LowerBandValue = lowerBandValue;
        }

        public int Period { get; }

        public decimal StandardDeviationMultiplier { get; }

        public decimal UpperBandValue { get; }

        public decimal MiddleAverageValue { get; }

        public decimal LowerBandValue { get; }

        private decimal? _bollingerBandWidth;
        public decimal BollingerBandWidth => _bollingerBandWidth ??= UpperBandValue - LowerBandValue;

        /// <summary>
        /// Returns a string to retrieve this indicator in the <see cref="CandleValue.Indicators"/> dictionary
        /// </summary>
        internal static string GetIndicatorString(int period, decimal standardDeviationMultiplier) => $"BollingerBand_{period}_{standardDeviationMultiplier}";
    }
}
