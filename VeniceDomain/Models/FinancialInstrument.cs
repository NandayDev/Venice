using System.Collections.Generic;
using VeniceDomain.Enums;

namespace VeniceDomain.Models
{
    public class FinancialInstrument
    {
        public string Ticker { get; set; }

        public string Isin { get; set; }

        public string CommercialName { get; set; }

        public FinancialInstrumentType InstrumentType { get; set; }

        public List<Dividend> Dividends { get; set; } = new List<Dividend>();

        public override bool Equals(object obj)
        {
            return obj is FinancialInstrument instrument &&
                   Isin == instrument.Isin;
        }

        public override int GetHashCode()
        {
            return 1576195124 + EqualityComparer<string>.Default.GetHashCode(Isin);
        }
    }
}
