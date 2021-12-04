using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain
{
    public static class VeniceMath
    {
        public static decimal GetVariance(this IEnumerable<decimal> values)
        {
            if (!values.Any())
            {
                return 0;
            }
            decimal avg = values.Average();
            decimal sum = values.Sum(v => (v - avg) * (v - avg));
            decimal denominator = values.Count() - 1;
            return sum / denominator;
        }

        public static decimal GetStandardDeviation(this IEnumerable<decimal> values)
        {
            if (!values.Any())
            {
                return 0;
            }
            return (decimal)Math.Sqrt((double)values.GetVariance());
        }
    }
}
