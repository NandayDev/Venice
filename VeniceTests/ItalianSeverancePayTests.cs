using VeniceDomain.Models;
using Xunit;

namespace VeniceTests
{
    public class ItalianSeverancePayTests
    {
        [Fact]
        public void TestSimpleSeverancePay()
        {
            // 2550€ di TFR nel 2020
            // 3400€ di TFR nel 2021

            // Nel 2020 l'inflazione è stata -0.3%
            // Quindi l'aumento del TFR è di 1,5% + (0,75 * -0,3) = 1,275%
            // A fine 2020 quindi avremo 2550 * 1,01275 = 2582,5125
            // Base imponibile uguale al lordo
            // Tasse al primo scaglione irpef 593.977875
            // Netto 1,988.53 €

            ItalianSeverancePaySummary summary = new ItalianSeverancePaySummary.Builder()
                .AddYearlySeverance(2020, 2550m)
                .AddYearlySeverance(2021, 3400m)
                .SetTargetYear(2021)
                .Build();

            Assert.Equal(1988.53m, summary.NetAmountAtTheEndOfLastYear, 2);

            // Nel 2021 l'inflazione è prevista al 2%
            // L'aumento del TFR è 1,5% + (0,75 * 2) = 3%
            // Fine 2021 saranno 2582,5125 (lordo 2020) + 3400 (lordo 2021) = 5982,5125 * 1,03 = 6161,9878
            // Netto 4,744.73 €

            Assert.Equal(6161.9878m, summary.GrossAmount, 2);
            Assert.Equal(4744.73m, summary.ExpectedNetAmountAtTheEndOfCurrentYear, 2);
        }
    }
}
