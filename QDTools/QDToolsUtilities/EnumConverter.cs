using System;

namespace QDToolsUtilities
{
    public static class CommandOptionEnumConverter
    {
        public static TEnum ToEnum<TEnum>(string value) where TEnum : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;

            return Enum.TryParse(value, true, out TEnum result) ? result : default;
        }
    }
}
