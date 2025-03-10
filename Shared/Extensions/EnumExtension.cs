using Shared.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Shared.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        DescriptionAttribute descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return null;
        }

        public static string GetColor(ColorType widgetColorType)
        {
            var css = widgetColorType switch
            {
                ColorType.Primary => "primary",
                ColorType.Secondary => "secondary",
                ColorType.Success => "success",
                ColorType.Danger => "danger",
                ColorType.Warning => "warning",
                ColorType.Info => "info",
                ColorType.Dark => "dark",
                _ => "primary"
            };
            return css;
        }
        public static string GetAverage(params int?[] a)
        {
            int sum = 0;
            foreach (var item in a)
            {
                if (item != null)
                    sum = sum + item.Value;
                else return "";
            }
            var count = Convert.ToDouble(a.Length);
            return (sum / count).ToString("0.##");
        }
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static DateTime GetTueyDate()
        {
            return new DateTime(2009, 7, 18);
        }

        public static T ToEnum<T>(string value) where T : Enum
        {
            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                if (GetDescription(enumValue).Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return enumValue;
                }
            }
            throw new ArgumentException($"'{value}' is not a valid value for enum {typeof(T).Name}");
        }
    }
}
