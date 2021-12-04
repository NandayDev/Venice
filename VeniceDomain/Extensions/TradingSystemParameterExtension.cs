using System.Collections.Generic;
using VeniceDomain.Models.TradingSystems;

namespace VeniceDomain.Extensions
{
    public static class TradingSystemParameterExtension
    {
        public static IEnumerable<decimal> GetAllPossibleValues(this TradingSystemParameter parameter)
        {
            if (parameter.MinValue == parameter.MaxValue)
            {
                yield return parameter.MinValue;
                yield break;
            }
            decimal value = parameter.MinValue;
            while(value <= parameter.MaxValue)
            {
                yield return value;
                value += parameter.Step;
            }
        }
    }
}
