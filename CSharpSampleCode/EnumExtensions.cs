using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CSharpSampleCode
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            // A much heavier weight approach would be to use the Humanizer package
            // [Humanize Enums]
            // (https://github.com/Humanizr/Humanizer?tab=readme-ov-file#humanize-enums)

            string valStr = enumValue.ToString();
            var displayAttribute = enumValue.GetType()
                .GetMember(valStr)
                .First()
                .GetCustomAttribute<DisplayAttribute>();
            string displayName =
                displayAttribute?.GetName() ?? valStr;
            return displayName;
        }
    }
}
