using VeniceDomain.Models.Base;

namespace VeniceDomain.Models
{
    public class CandleResponse : BaseTradingAction
    {
        private FinancialInstrument _financialInstrument;

        public override FinancialInstrument FinancialInstrument
        {
            get => _financialInstrument = _financialInstrument ?? CandleRequest.FinancialInstrument;
            set => _financialInstrument = value;
        }

        public virtual CandleRequest CandleRequest { get; set; }

        public virtual CandleValue CandleValue { get; set; }
    }
}
