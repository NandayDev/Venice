using System;
using System.Runtime.CompilerServices;

namespace VeniceDomain
{
    public static class ArgsValidationUtility
    {
        /// <summary>
        /// Throws an exception if given reference object is null
        /// </summary>
        public static void EnsureNotNull<T>(T obj)
            where T : class
        {
            if (obj is null)
                throw new ArgumentException(string.Format("{0} can't be null", obj.GetType().ToString()));
        }

        /// <summary>
        /// Throws an exception if given string is empty or null
        /// </summary>
        public static void EnsureStringNotEmptyNorNull(string s, [CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("{0} can't be empty, nor null", propertyName);
        }

        /// <summary>
        /// Throws an exception if given value object has default value
        /// </summary>
        public static void EnsureNotDefault<T>(T obj)
            where T : struct
        {
            if (obj.Equals(default(T)))
                throw new ArgumentException(string.Format("{0} can't have default value", obj.GetType().ToString()));
        }

        public static void EnsureInRange(decimal value, decimal startInclusive, decimal endInclusive, string argumentName)
        {
            if (value < startInclusive || value > endInclusive)
            {
                throw new ArgumentException($"{argumentName} must be >= {startInclusive} and <= {endInclusive}");
            }
        }
    }
}
