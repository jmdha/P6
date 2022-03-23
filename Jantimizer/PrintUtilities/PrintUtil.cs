using Konsole;

namespace PrintUtilities
{
    public class PrintUtil
    {
        public const ConsoleColor DefaultColor = ConsoleColor.White;
        private IConsole KConsole;
        private Dictionary<int, ProgressBar> progressbars = new Dictionary<int, ProgressBar>();

        public PrintUtil(IConsole console)
        {
            KConsole = console;
        }


        public void Print(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            KConsole.WriteLine(color, $"{GetIndent(indent)}{text}");
        }

        public void PrintLine(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            KConsole.WriteLine(color, $"{GetIndent(indent)}{text}");
        }

        public void PrintLine(List<string> text, List<string> format, List<ConsoleColor> colorsConsole, int indent = 0)
        {
            KConsole.Write($"{GetIndent(indent)}");
            for (int i = 0; i < text.Count; i++) {
                KConsole.Write(colorsConsole[i], format[i], text[i] + " ");
            }
            KConsole.WriteLine("");
        }

        public void PrintLine(List<string> text, List<string> format, ConsoleColor colorConsole, int indent = 0)
        {
            KConsole.Write($"{GetIndent(indent)}");
            for (int i = 0; i < text.Count; i++)
            {
                KConsole.Write(colorConsole, format[i], text[i] + " ");
            }
            KConsole.WriteLine("");
        }

        public void PrintInLine(string text, int indent = 0, ConsoleColor color = DefaultColor)
        {
            KConsole.Write(color, $"\r{GetIndent(indent)}{text}");
        }

        public int AddProgressBar(int max)
        {
            progressbars.Add(progressbars.Count + 1, new ProgressBar(KConsole, max));
            return progressbars.Count;
        }

        public void UpdateProgreesBar(int id, int current, string text)
        {
            progressbars[id].Refresh(current, text);
        }

        private string GetIndent(int indent)
        {
            return new string('\t', indent);
        }
    }
}