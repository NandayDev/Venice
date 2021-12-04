using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeniceDomain.Interfaces;
using VeniceDomain.Models;

namespace VeniceDomain.Services.TechnicalAnalysis
{
    public abstract class BaseIndicatorCalc<T> : IDisposable 
        where T : ITechnicalAnalysisIndicator
    {
        protected BaseIndicatorCalc(IEnumerable<CandleValue> candles) => Candles = candles.OrderBy(c => c.StartDate).ToList();

        protected List<CandleValue> Candles { get; }

        public abstract void GetAll();

        public virtual Task GetAllAsync() => Task.Run(GetAll);

        public void Dispose()
        {
        }
    }
}
