using System;
using System.Collections.Generic;
using System.Linq;
using VeniceDomain.Extensions;
using VeniceDomain.Models.TradingSystems;
using VeniceDomain.Utilities;

namespace VeniceDomain.Models
{
    public class TradingSystemReport
    {
        #region Constructor

        public TradingSystemReport(List<TradingOperation> operations, BrokerEnvironment brokerEnviroment,
            List<TradingSystemParameter> parameters, decimal currentAvailableFunds)
        {
            ArgsValidationUtility.EnsureNotNull(brokerEnviroment);
            Operations = operations;
            BrokerEnviroment = brokerEnviroment;
            Parameters = parameters.ToList().ConvertAll(p => p.Clone());
            StartingFunds = currentAvailableFunds;
            CurrentAvailableFunds = currentAvailableFunds;
            GetNetEarningAndDrawdown();
        }

        public TradingSystemReport(IEnumerable<TradingOperation> operations, BrokerEnvironment brokerEnviroment, List<TradingSystemParameter> parameters, decimal currentAvailableFunds)
            : this(new List<TradingOperation>(operations), brokerEnviroment, parameters, currentAvailableFunds)
        {
        }

        #endregion

        #region Instance properties

        public decimal NetEarning { get; private set; }

        public decimal NetEarningPercent { get; private set; }

        public decimal YearlyExpectedNetEarningPercent
        { 
            get
            {
                if (Operations.Count > 0)
                {
                    DateTime firstOperationDate = Operations[0].OpeningTransaction.ExecutionTime;
                    DateTime secondOperationDate = Operations[Operations.Count - 1].OpeningTransaction.ExecutionTime;
                    int daysDifference = (int)(secondOperationDate - firstOperationDate).TotalDays;
                    return NetEarningPercent / (daysDifference > 0 ? daysDifference : 1) * 365;
                }
                return 0;
            }
        }

        private List<decimal> drawdowns;

        public decimal MaxDrawdown => drawdowns.Min();

        public List<TradingSystemParameter> Parameters { get; set; }

        public virtual List<TradingOperation> Operations { get; }

        public virtual BrokerEnvironment BrokerEnviroment { get; }

        public bool FundsOver { get; private set; }

        public decimal StartingFunds { get; }

        private decimal CurrentAvailableFunds { get; set; }

        public List<(DateTime date, decimal availableFunds)> FundsOverTime { get; } = new List<(DateTime date, decimal availableFunds)>();

        #endregion

        #region Private methods

        private void GetNetEarningAndDrawdown()
        {
            //Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss:fffffff") + " - Start get net earning and drawdown");
            decimal initialFunds = CurrentAvailableFunds;
            drawdowns = new List<decimal>() { 0 };
            if (Operations.Count == 0)
            {
                return;
            }

            Transaction tradingResponseForTaxing = Operations[0].OpeningTransaction;
            // Draw downs list with a first element = 0 //
            foreach(TradingOperation operation in Operations)
            {
                // Funds update //
                FundsOverTime.Add((operation.OpeningTransaction.ExecutionTime, CurrentAvailableFunds));

                int quantity = (int)(CurrentAvailableFunds / operation.OpeningTransaction.ExecutionPrice);
                if (quantity <= 0)
                {
                    FundsOver = true;
                    break;
                }
                operation.OpeningTransaction.Quantity = quantity;
                operation.ClosingTransaction.Quantity = quantity;
                decimal operationNetResult = operation.NetResult;
                CurrentAvailableFunds += operationNetResult;
                NetEarning += operationNetResult;
                if (operationNetResult > 0)
                {
                    drawdowns.Add(drawdowns[^1]);
                }
                drawdowns[^1] += operationNetResult;
                // Funds update //
                FundsOverTime.Add((operation.ClosingTransaction.ExecutionTime, CurrentAvailableFunds));
            }
            NetEarningPercent = ((NetEarning + initialFunds) / initialFunds) - 1;
            //Debug.WriteLine(DateTime.Now.ToString("hh:mm:ss:fffffff") + " - End get net earning and drawdown");
        }

        #endregion
    }
}
