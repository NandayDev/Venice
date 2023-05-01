using System;

namespace VeniceDomain.Models.TradingSystems
{
    public class TradingSystemParameter
    {
        public TradingSystemParameter(string name, decimal currentValue)
        {
            Name = name;
            _currentValue = currentValue;
            MinValue = currentValue;
            MaxValue = currentValue;
        }

        public TradingSystemParameter(string name, bool currentValue)
            : this(name, currentValue ? 1 : 0)
        {
            Step = 1;
        }

        public TradingSystemParameter(string name, decimal minValue, decimal step, decimal maxValue)
        {
            ArgsValidationUtil.LowerThan(minValue, maxValue, true, name);
            Name = name;
            MinValue = minValue;
            Step = step;
            MaxValue = maxValue;
            _currentValue = minValue;
        }

        public string Name { get; set; }

        public decimal MinValue { get; set; }

        public decimal Step { get; set; }

        public decimal MaxValue { get; set; }

        private decimal _currentValue;
        public decimal CurrentValue 
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                OnCurrentValueChanged?.Invoke(value);
            }
        }

        public bool CurrentBooleanValue => CurrentValue == 1 ? true : (CurrentValue == 0 ? false : throw new ArgumentException());

        public event Action<decimal> OnCurrentValueChanged;

        public TradingSystemParameter Clone() => (MemberwiseClone() as TradingSystemParameter)!;
    }
}
