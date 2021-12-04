using System;

namespace VeniceDomain.Models
{
    public class Dividend 
    {
        public virtual FinancialInstrument FinancialInstrument { get; set; }

        public DateTime Date { get; set; }

        public decimal ValuePerUnit { get; set; }
    }
}
