using System;
using VeniceDomain.Enums;
using VeniceDomain.Extensions;

namespace VeniceDomain.Models
{
    public class PercentageCommissionPlan : CommissionPlan
    {
        public override BrokerCommissionPlanType BrokerCommissionType => BrokerCommissionPlanType.PURE_PERCENTUAL;

        public decimal CommissionPercentage { get; set; }

        public override decimal GetCommissionOnOperation(Transaction transaction) 
            => Math.Round(CommissionPercentage * transaction.GetTransactionTotalValue(), 2);
    }
}
