using System;

namespace VeniceDomain.Models
{
    public class Dividend 
    {
        public FinancialInstrument FinancialInstrument { get; set; }

        public DateTime Date { get; set; }

        public decimal ValuePerUnit { get; set; }
    }
}
