using System;

namespace VeniceDomain.Models
{
    public class Expense
    {
        public ExpenseCategory Category { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; }
    }
}
