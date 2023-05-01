using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain.Models
{
    public class AnnualSummary
    {
        private AnnualSummary(IEnumerable<IncomeEvent> incomeEvents, List<Expense> expenses, DateTime startingDate, DateTime referenceDate, int targetSavingRate, List<Expense> fixedExpenses)
        {
            IncomeEvents = incomeEvents;
            Expenses = expenses;
            StartingDate = startingDate;
            ReferenceDate = referenceDate;
            TargetSavingRate = targetSavingRate;
            FixedExpenses = fixedExpenses;
            Initialize();
        }

        public IEnumerable<IncomeEvent> IncomeEvents { get; }

        public List<Expense> Expenses { get; }

        public List<Expense> FixedExpenses { get; }

        public DateTime StartingDate { get; }

        public DateTime ReferenceDate { get; }

        public int TargetSavingRate { get; }

        public decimal CurrentBalance { get; private set; }

        public decimal DailyBudget { get; private set; }

        public decimal NeededDaysToRestoreBalance { get; private set; }

        public decimal PlannedMonthlySavingAmount { get; private set; }

        public decimal PlannedAnnualSavingAmount => PlannedMonthlySavingAmount * 12m;

        public decimal RealAnnualSavingAmount { get; private set; }

        public decimal RealMonthlySavingAmount => RealAnnualSavingAmount / 12m;

        public decimal PlannedAnnualBudget { get; private set; }

        public decimal PlannedMonthlyBudget => PlannedAnnualBudget / 12m;

        public decimal RealAnnualCostOfLiving { get; private set; }

        public decimal RealMonthlyCostOfLiving => RealAnnualCostOfLiving / 12m;

        public Dictionary<string, decimal> ExpensesPerCategory { get; } = new Dictionary<string, decimal>();

        public decimal ExpensesTotal { get; private set; }

        public decimal TotalEarnings { get; private set; }

        private void Initialize()
        {
            int daysBetweenStartingAndReferenceDate = (int)(ReferenceDate - StartingDate).TotalDays + 1;

            TotalEarnings = IncomeEvents.Sum(p => p.Amount);
            PlannedMonthlySavingAmount = TargetSavingRate / 1200m * TotalEarnings;
            PlannedAnnualBudget = TotalEarnings - PlannedAnnualSavingAmount - FixedExpenses.Sum(e => e.Amount);
            var expensesByCategory = Expenses.GroupBy(e => e.Category?.Name ?? "");
            foreach (IGrouping<string, Expense> group in expensesByCategory)
            {
                if (!ExpensesPerCategory.ContainsKey(group.Key))
                {
                    ExpensesPerCategory[group.Key] = 0m;
                }
                ExpensesPerCategory[group.Key] += group.Sum(g => g.Amount);
            }
            ExpensesTotal = ExpensesPerCategory.Sum(ec => ec.Value);

            int totalDaysInYear = DateTime.IsLeapYear(ReferenceDate.Year) ? 366 : 365;
            DailyBudget = PlannedAnnualBudget / totalDaysInYear;

            CurrentBalance = (DailyBudget * daysBetweenStartingAndReferenceDate) - ExpensesTotal;
            NeededDaysToRestoreBalance = CurrentBalance / DailyBudget;

            RealAnnualSavingAmount = PlannedAnnualSavingAmount + CurrentBalance;
            RealAnnualCostOfLiving = (ExpensesTotal / daysBetweenStartingAndReferenceDate * totalDaysInYear) + FixedExpenses.Sum(e => e.Amount);
        }

        public class Builder
        {
            private readonly List<IncomeEvent> _incomeEvents = new List<IncomeEvent>();
            private readonly List<Expense> _expenses = new List<Expense>();
            private DateTime _referenceDate = DateTime.Now.Date;
            private DateTime? _startingDate = null;
            private int _targetSavingRate = 50;
            private readonly List<Expense> _fixedExpenses = new List<Expense>();

            public Builder AddIncomeEvent(IncomeEvent incomeEvent)
            {
                ArgsValidationUtil.NotNull(incomeEvent, nameof(incomeEvent));
                _incomeEvents.Add(incomeEvent);
                return this;
            }

            public Builder AddIncomeEvents(IEnumerable<IncomeEvent> incomeEvents)
            {
                ArgsValidationUtil.NotNull(incomeEvents, nameof(incomeEvents));
                _incomeEvents.AddRange(incomeEvents);
                return this;
            }

            public Builder AddExpense(Expense expense)
            {
                ArgsValidationUtil.NotNull(expense, nameof(expense));
                _expenses.Add(expense);
                return this;
            }

            public Builder AddExpenses(IEnumerable<Expense> expenses)
            {
                ArgsValidationUtil.NotNull(expenses, nameof(expenses));
                _expenses.AddRange(expenses);
                return this;
            }

            public Builder SetStartingDate(DateTime startingDate)
            {
                _startingDate = startingDate;
                return this;
            }

            public Builder SetReferenceDate(DateTime referenceDate)
            {
                _referenceDate = referenceDate;
                return this;
            }

            public Builder SetTargetSavingRate(int targetSavingRate)
            {
                ArgsValidationUtil.InRange(targetSavingRate, 0, 100, nameof(targetSavingRate));
                _targetSavingRate = targetSavingRate;
                return this;
            }

            public Builder AddFixedExpense(Expense expense)
            {
                ArgsValidationUtil.NotNull(expense, nameof(expense));
                _fixedExpenses.Add(expense);
                return this;
            }

            public Builder AddFixedExpenses(IEnumerable<Expense> expenses)
            {
                ArgsValidationUtil.NotNull(expenses, nameof(expenses));
                _fixedExpenses.AddRange(expenses);
                return this;
            }

            public AnnualSummary Build()
            {
                if (_incomeEvents.Any(p => p.DateTime.Year != _referenceDate.Year))
                {
                    throw new ArgumentException("All income events must be from the same year of given reference date");
                }
                if (_startingDate == null)
                {
                    _startingDate = _referenceDate.AddDays(-_referenceDate.DayOfYear + 1);
                }
                return new AnnualSummary(_incomeEvents.OrderBy(p => p.DateTime), _expenses, _startingDate.Value, _referenceDate, _targetSavingRate, _fixedExpenses);
            }
        }
    }
}
