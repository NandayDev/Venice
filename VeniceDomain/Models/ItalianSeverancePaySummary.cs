using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain.Models
{
    public class ItalianSeverancePaySummary
    {
        private const int CURRENT_YEAR = 2021;

        private static readonly List<(int soglia, decimal tassazione)> soglieIrpef = new List<(int soglia, decimal tassazione)>
        {
            (15000, 0.23m),
            (28000, 0.27m),
            (55000, 0.38m),
            (75000, 0.41m),
            (int.MaxValue, 0.43m),
        };

        private ItalianSeverancePaySummary(Dictionary<int, decimal> severancesEachYear)
        {
            this.severancesEachYear = severancesEachYear;
            InitializeValues();
        }

        private readonly Dictionary<int, decimal> severancesEachYear = new Dictionary<int, decimal>();

        public decimal GrossAmount { get; private set; } = 0M;

        public decimal NetAmountAtTheEndOfLastYear { get; private set; }

        public decimal ExpectedNetAmountAtTheEndOfCurrentYear { get; private set; }

        private void InitializeValues()
        {
            int startingYear = severancesEachYear.Keys.Min();
            int currentYear = startingYear;
            while (currentYear < CURRENT_YEAR)
            {
                IncrementGrossAmountByYearlyValue(currentYear);
                currentYear += 1;
            }

            NetAmountAtTheEndOfLastYear = GetNetValue(currentYear);

            IncrementGrossAmountByYearlyValue(CURRENT_YEAR);
            ExpectedNetAmountAtTheEndOfCurrentYear = GetNetValue(CURRENT_YEAR);
        }

        private decimal GetYearlyInflation(int year)
        {
            return year switch
            {
                2010 => 0.016m,
                2011 => 0.027m,
                2012 => 0.030m,
                2013 => 0.011m,
                2014 => 0.002m,
                2015 => -0.001m,
                2016 => -0.001m,
                2017 => 0.011m,
                2018 => 0.011m,
                2019 => 0.005m,
                2020 => -0.003m,
                2021 => 0.020m,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void IncrementGrossAmountByYearlyValue(int year)
        {
            GrossAmount += severancesEachYear.GetValueOrDefault(year);
            decimal inflation = GetYearlyInflation(year);
            GrossAmount *= 1.015m + (0.75m * inflation);
        }

        private decimal GetValueForSogliaIrpef(decimal baseImponibile, (int soglia, decimal tassazione) sogliaIrpef, (int soglia, decimal tassazione)? sogliaPrecedente = null)
        {
            if (sogliaPrecedente != null)
            {
                if (baseImponibile < sogliaPrecedente.Value.soglia)
                {
                    return 0m;
                }
            }
            if (baseImponibile > sogliaIrpef.soglia)
            {
                return sogliaIrpef.soglia * sogliaIrpef.tassazione;
            }
            else
            {
                return baseImponibile * sogliaIrpef.tassazione;
            }
        }
        
        private decimal GetNetValue(int currentYear)
        {
            int startingYear = severancesEachYear.Keys.Min();
            int yearsOfWork = currentYear - startingYear;
            decimal baseImponibile;
            if ((GrossAmount * 12m / yearsOfWork) > GrossAmount)
            {
                baseImponibile = GrossAmount;
            }
            else
            {
                baseImponibile = GrossAmount * 12m;
            }
            decimal irpef1 = GetValueForSogliaIrpef(baseImponibile, soglieIrpef[0]);
            decimal irpef2 = GetValueForSogliaIrpef(baseImponibile, soglieIrpef[1], soglieIrpef[0]);
            decimal irpef3 = GetValueForSogliaIrpef(baseImponibile, soglieIrpef[2], soglieIrpef[1]);
            decimal irpef4 = GetValueForSogliaIrpef(baseImponibile, soglieIrpef[3], soglieIrpef[2]);
            decimal irpef5 = GetValueForSogliaIrpef(baseImponibile, soglieIrpef[4], soglieIrpef[3]);

            return GrossAmount - (irpef1 + irpef2 + irpef3 + irpef4 + irpef5);
        }

        public class Builder
        {
            private readonly Dictionary<int, decimal> severancesEachYear = new Dictionary<int, decimal>();

            public Builder AddYearlySeverance(int year, decimal amount)
            {
                if (year < 2010 || year > CURRENT_YEAR)
                {
                    throw new ArgumentOutOfRangeException("Year must be between 2010 and 2021");
                }
                if (!severancesEachYear.ContainsKey(year))
                {
                    severancesEachYear[year] = 0;
                }
                severancesEachYear[year] += amount;
                return this;
            }

            public ItalianSeverancePaySummary Build()
            {
                return new ItalianSeverancePaySummary(severancesEachYear);
            }
        }
    }
}
