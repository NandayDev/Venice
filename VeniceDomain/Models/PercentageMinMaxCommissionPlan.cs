using VeniceDomain.Enums;
using VeniceDomain.Extensions;

namespace VeniceDomain.Models
{
    public class PercentageMinMaxCommissionPlan : PercentageCommissionPlan
    {
        public override BrokerCommissionPlanType BrokerCommissionType => BrokerCommissionPlanType.PERCENTUAL_WITH_MIN_MAX;

        public decimal MinCommission { get; set; }

        public decimal MaxCommission { get; set; }

        private decimal MinTransactionValue => MinCommission / CommissionPercentage;

        private decimal MaxTransactionValue => MaxCommission / CommissionPercentage;

        public override decimal GetCommissionOnOperation(Transaction tradingResponse)
        {
            decimal transactionValue = tradingResponse.GetTransactionTotalValue();
            if (transactionValue < MinTransactionValue)
                // Below the minimum commission //
                return MinCommission;
            if (transactionValue > MaxTransactionValue)
                // Above the maximum commission //
                return MaxCommission;
            // Applying normal percentage commission //
            return base.GetCommissionOnOperation(tradingResponse);
        }
    }
}
