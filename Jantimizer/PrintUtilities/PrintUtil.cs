namespace PrintUtilities
{
    public static class PrintUtil
    {
        public static void Print(string text, int indent = 0, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write($"{GetIndent(indent)}{text}");
            Console.ResetColor();
        }

        public static void PrintLine(string text, int indent = 0, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{GetIndent(indent)}{text}");
            Console.ResetColor();
        }

        public static void PrintInLine(string text, int indent = 0, ConsoleColor color = ConsoleColor.White)
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