namespace VeniceDomain.Enums
{
    public enum TradingOrderType : byte
    {
        BUY = 0,
        SELL = 1,
        SHORT_SELL = 2,
        SHORT_BUY = 3
    }

    internal static class TradingOrderTypeExtension
    {
        internal static TradingSystemState GetTradingSystemState(this TradingOrderType orderType, TradingSystemState currentState)
        {
            switch (orderType)
            {
                case TradingOrderType.BUY:
                    return TradingSystemState.LONG;

                case TradingOrderType.SHORT_SELL:
                    return TradingSystemState.SHORT;

                case TradingOrderType.SELL:
                    return currentState switch
                    {
                        TradingSystemState.IDLE => TradingSystemState.IDLE,
                        TradingSystemState.LONG => TradingSystemState.IDLE,
                        TradingSystemState.SHORT => TradingSystemState.SHORT
                    };

                case TradingOrderType.SHORT_BUY:
                    return currentState switch
                    {
                        TradingSystemState.IDLE => TradingSystemState.IDLE,
                        TradingSystemState.LONG => TradingSystemState.LONG,
                        TradingSystemState.SHORT => TradingSystemState.IDLE
                    };
            }
            throw new System.Exception();
        }
    }
}
