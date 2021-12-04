using System;
using VeniceDomain.Properties;

namespace VeniceDomain.Extensions
{
    public static class GenericEnumExtension
    {
        public static string GetLocalizedString<T>(this T enumElement) where T : Enum
        {
            string translation = enumElement.ToString();
            string translationFromResources = EnumResources.ResourceManager.GetString(typeof(T).ToString() + "_" + enumElement.ToString());
            if (translationFromResources != null)
                translation = translationFromResources;
            return translation;
        }
    }
}
