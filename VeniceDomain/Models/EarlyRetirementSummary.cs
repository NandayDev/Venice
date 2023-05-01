using System;
using System.Collections.Generic;

namespace VeniceDomain.Models
{
    public class EarlyRetirementSummary
    {
        public EarlyRetirementSummary(int? earlyRetirementYear, decimal savingsAtRetirement)
        {
            EarlyRetirementYear = earlyRetirementYear;
            SavingsAtRetirement = savingsAtRetirement;
        }

        public int? EarlyRetirementYear { get; }

        public decimal SavingsAtRetirement { get; }

        public class Builder
        {
            private decimal? _savings;
            private int? _annualWorkReturnPercentage;
            private int? _annualRetirementReturnPercentage;
            private int? _savingRatePercentage;
            private decimal? _annualNetEarning;
            private int? _socialPensionStartingYear;
            private decimal? _netYearlySocialPensionAmount;
            private int? _presumedDeathYear;
            private readonly Dictionary<int, decimal> _additionalIncomeOrExpenses = new Dictionary<int, decimal>();
            private decimal? _annualSeverancePay;
            private int? _severancePayYearSince;
            private DateTime _referenceDate = DateTime.Now;

            public Builder SetReferenceDate(DateTime referenceDate)
            {
                _referenceDate = referenceDate;
                return this;
            }

            public Builder SetAnnualNetEarnings(decimal annualNetEarning)
            {
                ArgsValidationUtil.GreaterThan(annualNetEarning, 0, false, nameof(annualNetEarning));
                _annualNetEarning = annualNetEarning;
                return this;
            }

            /// <summary>
            /// Sets how much savings the user had at the end of last year <br/>
            /// From this year onward, net earnings * savingRate will be added to the algorithm
            /// </summary>
            public Builder SetEndOfLastYearSavings(decimal savings)
            {
                ArgsValidationUtil.GreaterThan(savings, 0, true, nameof(savings));
                _savings = savings;
                return this;
            }

            public Builder SetAnnualReturnOnInvestments(ReturnType returnType, int annualReturnPercentage)
            {
                ArgsValidationUtil.GreaterThan(annualReturnPercentage, 0, true, nameof(annualReturnPercentage));
                switch (returnType)
                {
                    case ReturnType.WHILE_WORKING:
                        _annualWorkReturnPercentage = annualReturnPercentage;
                        break;
                    case ReturnType.IN_RETIREMENT:
                        _annualRetirementReturnPercentage = annualReturnPercentage;
                        break;
                }
                return this;
            }

            public Builder SetSavingRate(int savingRatePercentage)
            {
                ArgsValidationUtil.InRange(savingRatePercentage, 0, 100, nameof(savingRatePercentage));
                _savingRatePercentage = savingRatePercentage;
                return this;
            }

            public Builder SetSocialPension(int year, decimal netYearlyAmount)
            {
                ArgsValidationUtil.InRange(year, _referenceDate.Year, _referenceDate.Year + 100, nameof(year));
                ArgsValidationUtil.GreaterThan(netYearlyAmount, 0, true, nameof(netYearlyAmount));
                _socialPensionStartingYear = year;
                _netYearlySocialPensionAmount = netYearlyAmount;
                return this;
            }

            public Builder SetPresumedDeathYear(int year)
            {
                ArgsValidationUtil.InRange(year, _referenceDate.Year, _referenceDate.Year + 200, nameof(year));
                _presumedDeathYear = year;
                return this;
            }

            public Builder AddExpenseInSpecificYear(int year, decimal expense)
            {
                ArgsValidationUtil.InRange(year, _referenceDate.Year, _referenceDate.Year + 200, nameof(year));
                ArgsValidationUtil.GreaterThan(expense, 0, true, nameof(expense));
                if (!_additionalIncomeOrExpenses.ContainsKey(year))
                {
                    _additionalIncomeOrExpenses[year] = 0m;
                }
                _additionalIncomeOrExpenses[year] -= expense;
                return this;
            }

            public Builder AddIncomeInSpecificYear(int year, decimal income)
            {
                ArgsValidationUtil.InRange(year, _referenceDate.Year, _referenceDate.Year + 200, nameof(year));
                ArgsValidationUtil.GreaterThan(income, 0, true, nameof(income));
                if (!_additionalIncomeOrExpenses.ContainsKey(year))
                {
                    _additionalIncomeOrExpenses[year] = 0m;
                }
                _additionalIncomeOrExpenses[year] += income;
                return this;
            }

            public Builder SetAnnualSeverancePay(decimal annualSeverancePay, int sinceYearInclusive)
            {
                ArgsValidationUtil.GreaterThan(annualSeverancePay, 0, true, nameof(annualSeverancePay));
                ArgsValidationUtil.InRange(sinceYearInclusive, _referenceDate.Year - 200, _referenceDate.Year, nameof(sinceYearInclusive));
                _annualSeverancePay = annualSeverancePay;
                _severancePayYearSince = sinceYearInclusive;
                return this;
            }

            public EarlyRetirementSummary Build()
            {
                EnsureNotNull(_savingRatePercentage, nameof(SetSavingRate));
                EnsureNotNull(_annualNetEarning, nameof(SetAnnualNetEarnings));
                EnsureNotNull(_annualWorkReturnPercentage, nameof(SetAnnualReturnOnInvestments));
                EnsureNotNull(_annualRetirementReturnPercentage, nameof(SetAnnualReturnOnInvestments));
                EnsureNotNull(_savings, nameof(SetEndOfLastYearSavings));
                EnsureNotNull(_socialPensionStartingYear, nameof(SetSocialPension));
                EnsureNotNull(_netYearlySocialPensionAmount, nameof(SetSocialPension));
                EnsureNotNull(_presumedDeathYear, nameof(SetPresumedDeathYear));
                int? retirementYear = null;
                decimal annualNetSavings = (_savingRatePercentage.Value / 100m) * _annualNetEarning.Value;
                decimal annualCostOfLiving = _annualNetEarning.Value * ((100 - _savingRatePercentage.Value) / 100m);
                decimal workReturnPercentage = 1 + (_annualWorkReturnPercentage.Value / 100m);
                decimal retirementReturnPercentage = 1 + (_annualRetirementReturnPercentage.Value / 100m);
                decimal savings = _savings.Value;
                for (int currentYear = _referenceDate.Year; currentYear < _socialPensionStartingYear.Value; currentYear += 1)
                {
                    // Adds app current year's savings to the savings total //
                    savings += annualNetSavings;
                    savings *= workReturnPercentage;
                    if (_additionalIncomeOrExpenses.ContainsKey(currentYear))
                    {
                        savings += _additionalIncomeOrExpenses[currentYear];
                    }
                    decimal severancePay = 0;
                    if (_annualSeverancePay != null)
                    {
                        ItalianSeverancePaySummary.Builder tfrBuilder = 
                            new ItalianSeverancePaySummary.Builder()
                                .SetTargetYear(currentYear);
                        for (int y = _severancePayYearSince.Value; y <= currentYear; y++)
                        {
                            tfrBuilder.AddYearlySeverance(y, _annualSeverancePay.Value);
                        }
                        severancePay = tfrBuilder.Build().ExpectedNetAmountAtTheEndOfCurrentYear;
                    }
                    savings += severancePay;
                    // Now tries to establish if savings are enough for retirement, through a cycle //
                    if (IsEnoughForRetirement(currentYear, savings, annualCostOfLiving, retirementReturnPercentage))
                    {
                        retirementYear = currentYear;
                        break;
                    }
                    else
                    {
                        savings -= severancePay;
                    }
                }

                return new EarlyRetirementSummary(retirementYear, savings);
            }

            private bool IsEnoughForRetirement(int currentYear, decimal savings, decimal annualCostOfLiving, decimal retirementReturnPercentage)
            {
                for (int year = currentYear; year <= _presumedDeathYear.Value; year++)
                {
                    savings -= annualCostOfLiving;
                    if (year > currentYear && _additionalIncomeOrExpenses.ContainsKey(year))
                    {
                        savings += _additionalIncomeOrExpenses[year];
                    }
                    if (year >= _socialPensionStartingYear.Value)
                    {
                        savings += _netYearlySocialPensionAmount.Value;
                    }
                    if (savings < 0)
                    {
                        // Savings weren't enough for retirement  //
                        return false;
                    }
                    savings *= retirementReturnPercentage;
                }
                return true;
            }

            private void EnsureNotNull<T>(T? value, string methodName) where T : struct
            {
                if (value == null)
                {
                    throw new ArgumentException($"{methodName}() must be called before calling {nameof(Build)}()");
                }
            }
        }

        public enum ReturnType
        {
            WHILE_WORKING,
            IN_RETIREMENT
        }
    }
}
