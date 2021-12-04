using System;

namespace VeniceDomain
{
    public static class ArgsValidationUtil
    {
        public static void NotNull(object o, string parameterName)
        {
            if (o is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ArrayNotEmpty<T>(T[] array, string parameterName)
        {
            if (array.Length == 0)
            {
                throw new ArgumentException($"{parameterName} can't be an empty array");
            }
        }

        public static void NotZero(decimal number, string parameterName)
        {
            if (number == 0)
            {
                throw new ArgumentException($"{parameterName} can't be zero");
            }
        }

        public static void InRange(int number, int minimumInclusive, int maximumInclusive, string parameterName)
        {
            if (number < minimumInclusive || number > maximumInclusive)
            {
                throw new ArgumentException($"{parameterName} must be >= {minimumInclusive} and <= {maximumInclusive}");
            }
        }
    }
}
