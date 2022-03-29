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

        public static T GetRegexVal<T>(Match regexMatch, string name) where T : IConvertible
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!regexMatch.Success)
                throw new ArgumentException("Regex wasn't matched");

            if (!regexMatch.Groups[name].Success)
                throw new ArgumentException($"Regex not matched for group '{name}'");


            return (T)Convert.ChangeType(regexMatch.Groups[name].Value, typeof(T), CultureInfo.InvariantCulture);
        }


        public static T? GetRegexValNullable<T>(Match regexMatch, string name) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (!regexMatch.Success)
                throw new ArgumentException("Regex wasn't matched");

            if (!regexMatch.Groups[name].Success)
                return null;


            return (T)Convert.ChangeType(regexMatch.Groups[name].Value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
