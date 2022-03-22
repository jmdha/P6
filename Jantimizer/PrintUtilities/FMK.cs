using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintUtilities
{
    static public class FMK
    {
        public static IEnumerable<int> PrintProgress(int max)
        {
            return PrintProgress(0, max);
        }

        public static IEnumerable<int> PrintProgress(int min, int max)
        {
            for (int i = min; i <= max; i++)
            {
                yield return i;
            }
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
