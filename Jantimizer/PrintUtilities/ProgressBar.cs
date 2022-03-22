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
        public const int RainbowSize = 4;
        private static List<Color> rainbow = new List<Color>();
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
            Color startColor = Color.Red;
            Color endColor = Color.Green;

            int value = (int)(((float)current / (float)max) * width);
            if (inline)
                PrintUtil.PrintInLine("");
            PrintUtil.Print("[", indent, ConsoleColor.DarkGray);
            for (int i = 0; i < width; i++)
            {
                if (i < value)
                    PrintUtil.Print("■", 0, FromColor(GetGradient(i)));
                else
                    PrintUtil.Print(" ", 0);
            }
            PrintUtil.Print($"] ({current}/{max})", 0, ConsoleColor.DarkGray);
        }

        public static System.ConsoleColor FromColor(System.Drawing.Color c)
        {
            int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
            index |= (c.R > 64) ? 4 : 0; // Red bit
            index |= (c.G > 64) ? 2 : 0; // Green bit
            index |= (c.B > 64) ? 1 : 0; // Blue bit
            return (System.ConsoleColor)index;
        }

        public static Color GetGradient(int i)
        {
            if (rainbow.Count == 0)
                GenerateRainbow();
            return (Color)rainbow[(i + 1) % RainbowSize];
        }

        public static void GenerateRainbow()
        {
            for (double i = 0; i < 1; i += 1/((double)RainbowSize))
            {
                rainbow.Add(ColorRGB.HSL2RGB(i, 0.5, 0.5));
            }
        }
    }
}
