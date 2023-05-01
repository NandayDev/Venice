using VeniceDomain.Enums;

namespace VeniceDomain.Models
{
    public class FixedCommissionPlan : CommissionPlan
    {
        public FixedCommissionPlan(decimal fixedCommission)
        {
            _fixedCommission = fixedCommission;
        }

        private readonly decimal _fixedCommission;

        public override BrokerCommissionPlanType BrokerCommissionType { get; } = BrokerCommissionPlanType.PURE_FIXED;

        public override decimal GetCommissionOnOperation(Transaction tradingResponse) => _fixedCommission;
    }
}
