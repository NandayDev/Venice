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

        /// <summary>
        /// Throws an exception if given <paramref name="number"/> is not in given range
        /// </summary>
        public static void InRange(int number, int minimumInclusive, int maximumInclusive, string parameterName)
        {
            if (number < minimumInclusive || number > maximumInclusive)
            {
                throw new ArgumentException($"{parameterName} must be >= {minimumInclusive} and <= {maximumInclusive}");
            }
        }
        
        /// <summary>
        /// Throws an exception if given <paramref name="number"/> is not in given range
        /// </summary>
        public static void InRange(decimal number, decimal minimumInclusive, decimal maximumInclusive, string parameterName)
        {
            if (number < minimumInclusive || number > maximumInclusive)
            {
                throw new ArgumentException($"{parameterName} must be >= {minimumInclusive} and <= {maximumInclusive}");
            }
        }

        public static void InRange(
            decimal number, 
            string parameterName, 
            decimal? minimumInclusive = null, 
            decimal? minimumExclusive = null, 
            decimal? maximumInclusive = null, 
            decimal? maximumExclusive = null)
        {
            if (minimumInclusive != null)
            {
                if (number < minimumInclusive)
                {
                    throw new ArgumentException($"{parameterName} must be >= {minimumInclusive}");
                }
            }
            if (minimumExclusive != null)
            {
                if (number <= minimumExclusive)
                {
                    throw new ArgumentException($"{parameterName} must be > {minimumExclusive}");
                }
            }
            if (maximumInclusive != null)
            {
                if (number > maximumInclusive)
                {
                    throw new ArgumentException($"{parameterName} must be <= {maximumInclusive}");
                }
            }
            if (maximumExclusive != null)
            {
                if (number >= maximumExclusive)
                {
                    throw new ArgumentException($"{parameterName} must be < {maximumExclusive}");
                }
            }
        }

        public static void GreaterThan(decimal number, decimal minimum, bool inclusive, string parameterName)
        {
            if (inclusive)
            {
                if (number < minimum)
                {
                    throw new ArgumentException($"{parameterName} must be greater or equal to {minimum}");
                }
            }
            else
            {
                if (number <= minimum)
                {
                    throw new ArgumentException($"{parameterName} must be greater than {minimum}");
                }
            }
        }

        public static void LowerThan(decimal number, decimal maximum, bool inclusive, string parameterName)
        {
            if (inclusive)
            {
                if (number > maximum)
                {
                    throw new ArgumentException($"{parameterName} must be lower or equal to {maximum}");
                }
            }
            else
            {
                if (number >= maximum)
                {
                    throw new ArgumentException($"{parameterName} must be lower than {maximum}");
                }
            }
        }
    }
}
