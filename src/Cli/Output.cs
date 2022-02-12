using snipesharp.Cli;
using StringExtensions;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            var consoleWidth = Console.WindowWidth;

            // construct crosshair string
            var crosshair = "";
            int crosshairLength = (consoleWidth % 2 != 0) ? consoleWidth : consoleWidth - 1;
            for (int i = 0; i < crosshairLength; i++) crosshair += i == consoleWidth / 2 ? "+" : "-";

            // print
            Console.WriteLine("".Centered().Cross());
            Console.WriteLine(SetText.LightCyan + @"   ____ , __   ` \,___,   ___    ____ /        __   .___  \,___,".Centered().Cross());
            Console.WriteLine(SetText.Cyan + @"  (     |'  `. | |    \ .'  _/  (     |,---.  /  `. /   \ |    \".Centered().Cross());
            Console.WriteLine(SetText.DarkRed + crosshair.Substring(0,crosshair.Length-1));
            Console.WriteLine(SetText.Blue + @" \___.' /    | / |`---' `.___, \___.' /    | `.__/| /     |`---'".Centered().Cross());
            Console.WriteLine(SetText.DarkBlue + @"                 \                                        \     ".Centered().Cross());
            Console.WriteLine("".Centered().Cross());
        }

        public static void Inform(string message){
            Console.WriteLine($"{SetText.Blue}[info]{SetText.ResetAll} {message}");
        }

        public static void Warn(string message){
            Console.WriteLine($"{SetText.Yellow}[warning]{SetText.ResetAll} {message}");
        }

        public static void Error(string message){
            Console.WriteLine($"{SetText.Red}[error]{SetText.ResetAll} {message}");
        }

        public static void Input(string message){
            Console.Write($"{SetText.Blue}[input]{SetText.ResetAll} {message}: ");
        }
    }
}