using System;

namespace VeniceDomain.Models
{
    /// <summary>
    /// An event of income, from any source, job, gift
    /// </summary>
    public class IncomeEvent
    {
        /// <summary>
        /// Date in which this income event was received
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Identification name for this income, nullable
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Amount of the income event
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Category to which this income event belongs
        /// </summary>
        public IncomeCategory? Category { get; set; }
    }
}
