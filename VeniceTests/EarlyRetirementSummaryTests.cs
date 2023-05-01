using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class EarlyRetirementSummaryTests
    {
        [Fact]
        public void TestEarlyRetirement()
        {
            var builder = new EarlyRetirementSummary.Builder();
            Assert.Throws<ArgumentException>(() => builder.Build());
            
            builder.SetAnnualReturnOnInvestments(EarlyRetirementSummary.ReturnType.WHILE_WORKING, 3);
            Assert.Throws<ArgumentException>(() => builder.Build());
            builder.SetAnnualReturnOnInvestments(EarlyRetirementSummary.ReturnType.IN_RETIREMENT, 2);
            Assert.Throws<ArgumentException>(() => builder.Build());

            builder.SetEndOfLastYearSavings(53000);
            Assert.Throws<ArgumentException>(() => builder.Build());

            builder.SetAnnualNetEarnings(2700 * 13);
            Assert.Throws<ArgumentException>(() => builder.Build());

            builder.SetSavingRate(50);
            Assert.Throws<ArgumentException>(() => builder.Build());

            builder.SetSocialPension(2060, 18000);
            Assert.Throws<ArgumentException>(() => builder.Build());

            builder.SetReferenceDate(new DateTime(2021, 11, 30));
            builder.SetPresumedDeathYear(2080);

            builder
                .AddExpenseInSpecificYear(2030, 20000)
                .AddExpenseInSpecificYear(2040, 20000)
                .AddExpenseInSpecificYear(2050, 20000)
                .AddExpenseInSpecificYear(2060, 20000)
                .AddExpenseInSpecificYear(2070, 20000);

            EarlyRetirementSummary earlyRetirementSummary = builder.Build();

            Assert.NotNull(earlyRetirementSummary.EarlyRetirementYear);
            Assert.Equal(2036, earlyRetirementSummary.EarlyRetirementYear.Value);
            Assert.Equal(425534.26m, earlyRetirementSummary.SavingsAtRetirement, 2);

            builder.AddIncomeInSpecificYear(earlyRetirementSummary.EarlyRetirementYear.Value - 2, (earlyRetirementSummary.EarlyRetirementYear.Value - 2021 - 2) * 2500m);
            earlyRetirementSummary = builder.Build();

            Assert.NotNull(earlyRetirementSummary.EarlyRetirementYear);
            Assert.Equal(2035, earlyRetirementSummary.EarlyRetirementYear.Value);
            Assert.Equal(429065.06m, earlyRetirementSummary.SavingsAtRetirement, 2);
        }
    }
}
