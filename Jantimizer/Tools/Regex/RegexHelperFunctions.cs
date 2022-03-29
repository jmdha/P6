using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tools.Regex
{
    public static class RegexHelperFunctions
    {

        public static T GetRegexVal<T>(Match regexMatch, string name)
        {

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Type nonNullableT = ToNotNullable(typeof(T));

            if (!regexMatch.Success)
                throw new ArgumentException("Regex wasn't matched");

            if (!regexMatch.Groups[name].Success)
            {
                if (typeof(T) != nonNullableT) // If T is nullable
                    return default(T)!; // I assume it's null, but compiler wont accept 'null'
                else
                    throw new ArgumentException($"Regex not matched for group '{name}'");
            }


            return (T)Convert.ChangeType(regexMatch.Groups[name].Value, nonNullableT, CultureInfo.InvariantCulture);
        }

        private static Type ToNotNullable(Type T)
        {
            // Underlying is null if T was non-nullable
            Type? underlying = Nullable.GetUnderlyingType(T);
            if (underlying != null)
                return underlying;

            return T;
        }
    }
}
