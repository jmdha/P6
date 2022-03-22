namespace PrintUtilities
{
    public static class PrintUtil
    {
        public const ConsoleColor DefaultColor = ConsoleColor.White;
        public static void Print(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            Console.ForegroundColor = color;
            Console.Write($"{GetIndent(indent)}{text}");
            Console.ResetColor();
        }

        public static void PrintLine(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{GetIndent(indent)}{text}");
            Console.ResetColor();
        }

        public static void PrintLine(List<string> text, List<string> format, List<ConsoleColor> colorsConsole, int indent = 0)
        {
            Console.Write($"{GetIndent(indent)}");
            for (int i = 0; i < text.Count; i++) {
                Print(String.Format(format[i], text[i]) + " ", 0, colorsConsole[i]);
            }
            Console.WriteLine();
            Console.ResetColor();
        }

        public static void PrintInLine(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            Console.ForegroundColor = color;
            Console.Write($"\r{GetIndent(indent)}{text}");
            Console.ResetColor();
        }

        public static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new String(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static string GetIndent(int indent)
        {
            return new string('\t', indent);
        }
    }
}