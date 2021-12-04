using System.Collections.Generic;

namespace VeniceDomain.Models
{
    public class ExpenseCategory
    {
        public string Name { get; set; }

        public virtual List<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
