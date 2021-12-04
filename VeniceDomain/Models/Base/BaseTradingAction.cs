namespace VeniceDomain.Models.Base
{
    public abstract class BaseTradingAction
    {
        public virtual FinancialInstrument FinancialInstrument { get; set; }
    }
}
