using System;
using VeniceDomain.Enums;

namespace VeniceDomain.Extensions
{
    internal static class TradingOrderTypeExtension
    {
        internal static bool IsFirstOrderType(this TradingOrderType orderType)
        {
            switch (orderType)
            {
                case TradingOrderType.BUY:
                case TradingOrderType.SHORT_SELL:
                    return true;
                case TradingOrderType.SELL:
                case TradingOrderType.SHORT_BUY:
                    return false;
            }
            throw new ArgumentException();
        }
    }
}
