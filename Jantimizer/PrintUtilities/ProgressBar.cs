using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintUtilities
{
    public static class ProgressBar
    {
        public const int DefaultWidth = 100;
        private static List<ConsoleColor> rainbow = new List<ConsoleColor>();
        public static IEnumerable<int> PrintProgress(int max, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            return PrintProgress(0, max, width, inline, indent);
        }

        public static IEnumerable<int> PrintProgress(int min, int max, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            for (int i = min; i < max; i++)
            {
                PrintProgressBar(i, max, width, inline, indent);
                yield return i;
            }
        }

        public static IEnumerable<T> PrintProgress<T>(IEnumerable<T> ts, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            int i = 0;
            foreach (T t in ts)
            {
                PrintProgressBar(i, ts.Count(), width, inline, indent);
                i++;
                yield return t;
            }
        }

        public static async IAsyncEnumerable<T> PrintProgress<T>(IAsyncEnumerable<T> ts, int max, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            int i = 0;
            await foreach (T t in ts)
            {
                PrintProgressBar(i, max, width, inline, indent);
                i++;
                yield return t;
            }
        }

        public static void Finish(int max, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            PrintUtil.ClearLine();
            PrintProgressBar(max, max, width, inline, indent);
            PrintUtil.PrintLine(" finished!", color: ConsoleColor.Green);
        }
        
        private static void PrintProgressBar(int current, int max, int width, bool inline, int indent = 0)
        {
            Color startColor = Color.Red;
            Color endColor = Color.Green;

            int value = (int)(((float)current / (float)max) * width);
            if (inline)
                PrintUtil.PrintInLine("");
            PrintUtil.Print("[", indent, ConsoleColor.DarkGray);
            for (int i = 0; i < width; i++)
            {
                if (i < value)
                    PrintUtil.Print("■", 0, GetGradient(i));
                else
                    PrintUtil.Print(" ", 0);
            }
            PrintUtil.Print($"] ({current}/{max})", 0, ConsoleColor.DarkGray);
        }

        public static ConsoleColor GetGradient(int i)
        {
            if (rainbow.Count == 0)
                GenerateRainbow();
            return rainbow[i % rainbow.Count];
        }

        public static void GenerateRainbow()
        {
            rainbow.Add(ConsoleColor.Yellow);
            rainbow.Add(ConsoleColor.DarkYellow);
            rainbow.Add(ConsoleColor.Red);
            rainbow.Add(ConsoleColor.DarkMagenta);
            rainbow.Add(ConsoleColor.Magenta);
            rainbow.Add(ConsoleColor.DarkBlue);
            rainbow.Add(ConsoleColor.Blue);
            rainbow.Add(ConsoleColor.Cyan);
            rainbow.Add(ConsoleColor.DarkCyan);
            rainbow.Add(ConsoleColor.DarkBlue);
            rainbow.Add(ConsoleColor.Blue);
            rainbow.Add(ConsoleColor.DarkGreen);
            rainbow.Add(ConsoleColor.Green);
        }
    }
}
