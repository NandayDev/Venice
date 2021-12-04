namespace VeniceDomain
{
    public enum TransactionType
    {
        /// <summary>
        /// Default value: not defined
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// A transaction made for investment purposes
        /// </summary>
        Investment = 1,

        /// <summary>
        /// A transaction made for trading purposes
        /// </summary>
        Free_Trading = 2,

        /// <summary>
        /// A transaction made to follow a trading system
        /// </summary>
        Trading_System = 3
    }
}
