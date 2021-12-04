using VeniceDomain.Enums;

namespace VeniceDomain.Models
{
    public abstract class CommissionPlan
    {
        public abstract BrokerCommissionPlanType BrokerCommissionType { get; }

        public abstract decimal GetCommissionOnOperation(Transaction tradingResponse);

        public virtual BrokerEnvironment BrokerEnviroment { get; set; }
    }
}
