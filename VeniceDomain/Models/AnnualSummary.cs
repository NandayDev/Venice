using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain.Models
{
    public class AnnualSummary
    {
        private AnnualSummary(IEnumerable<Paycheck> paychecks, List<Expense> expenses, DateTime startingDate, DateTime referenceDate, int targetSavingRate, Dictionary<int, decimal> otherGains, List<Expense> fixedExpenses)
        {
            Paychecks = paychecks;
            Expenses = expenses;
            StartingDate = startingDate;
            ReferenceDate = referenceDate;
            TargetSavingRate = targetSavingRate;
            FixedExpenses = fixedExpenses;
            _otherGainsPerMonth = otherGains;
            Initialize();
        }

        private readonly Dictionary<int, decimal> _otherGainsPerMonth;

        public IEnumerable<Paycheck> Paychecks { get; }

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

        public Dictionary<string, decimal> ExpensesPerCategory { get; private set; }

        public Dictionary<int, decimal> EarningsPerMonth { get; private set; }

        public decimal ExpensesTotal { get; private set; }

        public decimal TotalEarnings { get; private set; }

        private void Initialize()
        {
            EarningsPerMonth = new Dictionary<int, decimal>();
            for (int i = 1; i <= 12; i++)
            {
                EarningsPerMonth[i] = 0m;
                Paycheck monthPaycheck = Paychecks.SingleOrDefault(p => p.Month == i);
                if (monthPaycheck != null)
                {
                    EarningsPerMonth[i] += monthPaycheck.NetSalary;
                }
                else
                {
                    EarningsPerMonth[i] += Paychecks.ElementAtOrDefault(i - 1)?.NetSalary ?? Paychecks.Average(p => p.NetSalary);
                }
                if (_otherGainsPerMonth.TryGetValue(i, out decimal gain))
                {
                    EarningsPerMonth[i] += gain;
                }
            }

            Paycheck tredicesima = Paychecks.SingleOrDefault(p => p.Month == 13);
            if (tredicesima != null)
            {
                EarningsPerMonth[12] += tredicesima.NetSalary;
            }

            int daysBetweenStartingAndReferenceDate = (int)(ReferenceDate - StartingDate).TotalDays + 1;

            TotalEarnings = EarningsPerMonth.Sum(p => p.Value);
            PlannedMonthlySavingAmount = TargetSavingRate / 1200m * TotalEarnings;
            PlannedAnnualBudget = TotalEarnings - PlannedAnnualSavingAmount - FixedExpenses.Sum(e => e.Amount);
            ExpensesPerCategory = new Dictionary<string, decimal>();
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
            private readonly List<Paycheck> _paychecks = new List<Paycheck>();
            private readonly List<Expense> _expenses = new List<Expense>();
            private DateTime _referenceDate = DateTime.Now.Date;
            private DateTime? _startingDate = null;
            private int _targetSavingRate = 50;
            private readonly List<Expense> _fixedExpenses = new List<Expense>();
            private readonly Dictionary<int, decimal> _otherGainsPerMonth = new Dictionary<int, decimal>();

            public Builder AddPaycheck(Paycheck paycheck)
            {
                ArgsValidationUtil.NotNull(paycheck, nameof(paycheck));
                _paychecks.Add(paycheck);
                return this;
            }

            public Builder AddPaychecks(IEnumerable<Paycheck> paychecks)
            {
                ArgsValidationUtil.NotNull(paychecks, nameof(paychecks));
                _paychecks.AddRange(paychecks);
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

            public Builder AddGain(DateTime date, decimal amount)
            {
                ArgsValidationUtil.NotZero(amount, nameof(amount));
                if (!_otherGainsPerMonth.ContainsKey(date.Month))
                {
                    _otherGainsPerMonth.Add(date.Month, 0m);
                }
                _otherGainsPerMonth[date.Month] += amount;
                return this;
            }

            public AnnualSummary Build()
            {
                if (_paychecks.Any(p => p.Year != _referenceDate.Year))
                {
                    throw new ArgumentException("All paychecks must be from the same year of given reference date");
                }
                if (_startingDate == null)
                {
                    _startingDate = _referenceDate.AddDays(-_referenceDate.DayOfYear + 1);
                }
                return new AnnualSummary(_paychecks.OrderBy(p => p.Month), _expenses, _startingDate.Value, _referenceDate, _targetSavingRate, _otherGainsPerMonth, _fixedExpenses);
            }
        }
    }
}
