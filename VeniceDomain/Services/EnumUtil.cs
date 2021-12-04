using System;
using System.Collections.Generic;
using System.Linq;

namespace VeniceDomain.Services
{
    public static class EnumUtil
    {
        /// <summary>
        /// Returns an IEnumerable with all elements of provided <typeparamref name="TEnum"/> type
        /// </summary>
        public static IEnumerable<TEnum> GetAll<TEnum>()
            where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static bool TryParseFromInteger<TEnum>(int integer, out TEnum parsedEnum)
            where TEnum : Enum
        {
            foreach (TEnum element in GetAll<TEnum>())
            {
                if (Convert.ToInt32(element) == integer)
                {
                    parsedEnum = element;
                    return true;
                }
            }
            parsedEnum = default;
            return false;
        }
    }
}
