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
            // The initial implementation came from 
            // https://dnilvincent.net/blog/posts/how-to-get-enum-display-name-in-csharp-net

            // A much more heavy weight and elegant approach would be to use
            // the Humanizer package [Humanize Enums]
            // (https://github.com/Humanizr/Humanizer?tab=readme-ov-file#humanize-enums)
            // I am also unsure how it would handle the invalid value case.

            string valStr = enumValue.ToString();
            string displayName;
            try
            {
                MemberInfo[] memberInfos = enumValue.GetType().GetMember(valStr);
                MemberInfo memberInfo = memberInfos.First();
                var displayAttr =
                    memberInfo?.GetCustomAttribute<DisplayAttribute>();
                displayName = displayAttr?.GetName() ?? valStr;
            }
            catch (System.InvalidOperationException)
            {
                // get here when an int value that is not one of the enum
                // values is cast to an enum.
                displayName = $"INVALID({valStr})";
            }

            return displayName;
        }
    }
}