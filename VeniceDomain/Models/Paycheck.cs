using System;

namespace VeniceDomain.Models
{
    /// <summary>
    /// Class representing the monthly pay check received
    /// </summary>
    public class Paycheck
    {
        /// <summary>
        /// Paycheck's year reference
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Paycheck's month reference (1-13)<br/>
        /// 13 is the "tredicesima"
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gross salary, by contract
        /// </summary>
        public decimal GrossSalary { get; set; }
        
        /// <summary>
        /// Effective gross salary, with overtime and other included
        /// </summary>
        public decimal GrossSalaryPlusOvertime { get; set; }

        /// <summary>
        /// Count of overtime hours
        /// </summary>
        public decimal OvertimeHoursCount { get; set; }

        /// <summary>
        /// Days of vacation left
        /// </summary>
        public decimal VacationDaysLeft { get; set; }

        /// <summary>
        /// Leave hours left
        /// </summary>
        public decimal LeaveHoursLeft { get; set; }

        /// <summary>
        /// Net salary received in the bank account
        /// </summary>
        public decimal NetSalary { get; set; }

        /// <summary>
        /// Severance pay accumulated this month
        /// </summary>
        public decimal SeverancePay { get; set; }

        /// <summary>
        /// Whether this paycheck is effectively complete, or partial
        /// </summary>
        public bool IsComplete { get; set; }
    }
}
