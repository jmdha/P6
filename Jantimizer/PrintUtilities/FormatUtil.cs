using System.Text;

namespace PrintUtilities
{
    public static class FormatUtil
    {
        public const ConsoleColor DefaultColor = ConsoleColor.White;
        public static string Print(string text)
        {
            return $"{text}";
        }

        public static string PrintLine(string text)
        {
            return $"{text}{Environment.NewLine}";
        }

        public static string PrintLine(List<string> text, List<string> format)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < text.Count; i++) {
                sb.Append(Print(String.Format(format[i], text[i]) + " "));
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}