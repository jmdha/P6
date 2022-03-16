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

        public static void PrintProgressBar(int current, int max, int width, bool inline, int indent = 0)
        {
            int value = (int)(((float)current / (float)max) * width);
            if (inline)
                PrintInLine("");
            Print("[", indent, ConsoleColor.DarkGray);
            for (int i = 0; i < width; i++)
            {
                if (i < value)
                    Print("X", 0, ConsoleColor.White);
                else
                    Print(" ", 0);
            }
            Print($"] ({current}/{max})", 0, ConsoleColor.DarkGray);
        }

        private static string GetIndent(int indent)
        {
            return new string('\t', indent);
        }
    }
}