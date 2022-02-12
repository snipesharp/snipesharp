using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringExtensions
{
    public static class SetText
    {
        // Colors

        public static string Black = "\x1b[30m";

        public static string DarkRed = "\x1b[31m";

        public static string DarkGreen = "\x1b[32m";

        public static string DarkYellow = "\x1b[33m";

        public static string DarkBlue = "\x1b[34m";

        public static string DarkMagenta = "\x1b[35m";

        public static string Cyan = "\x1b[36m";

        public static string White = "\x1b[37m";

        public static string Gray = "\x1b[90m";

        public static string Red = "\x1b[91m";

        public static string Green = "\x1b[92m";

        public static string Yellow = "\x1b[93m";

        public static string Blue = "\x1b[94m";

        public static string Magenta = "\x1b[95m";

        public static string LightCyan = "\x1b[96m";

        public static string LightWhite = "\x1b[97m";

        // Other

        public static string ResetAll = "\x1b[0m";

        public static string Bold = "\x1b[1m";

        public static string BoldOff = "\x1b[21m";

        public static string Underline = "\x1b[4m";

        public static string UnderlineOff = "\x1b[24m";

        public static string Blink = "\x1b[5m";

        public static string BlinkOff = "\x1b[25m";
    }
}
