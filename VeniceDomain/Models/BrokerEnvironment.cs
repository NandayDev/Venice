namespace VeniceDomain.Models
{
    public class BrokerEnvironment
    {
        public string Name { get; set; }

        public virtual CommissionPlan CommissionPlan { get; set; }

        public decimal NetEarningTaxation { get; set; }

        public decimal AnnualTax { get; set; }
    }
}
