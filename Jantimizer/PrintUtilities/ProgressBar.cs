using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintUtilities
{
    public static class ProgressBar
    {
        public const int DefaultWidth = 50;
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

        public static void Finish(int max, int width = DefaultWidth, bool inline = true, int indent = 0)
        {
            PrintUtil.ClearLine();
            PrintProgressBar(max, max, width, inline, indent);
            PrintUtil.PrintLine(" finished!", color: ConsoleColor.Green);
        }

        private static void PrintProgressBar(int current, int max, int width, bool inline, int indent = 0)
        {
            int value = (int)(((float)current / (float)max) * width);
            if (inline)
                PrintUtil.PrintInLine("");
            PrintUtil.Print("[", indent, ConsoleColor.DarkGray);
            for (int i = 0; i < width; i++)
            {
                if (i < value)
                    PrintUtil.Print("X", 0, ConsoleColor.White);
                else
                    PrintUtil.Print(" ", 0);
            }
            PrintUtil.Print($"] ({current}/{max})", 0, ConsoleColor.DarkGray);
        }
    }
}
