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
            Console.WriteLine(@"   ____ , __   ` \,___,   ___    ____ /        __   .___  \,___,".Centered().Cross());
            Console.WriteLine(@"  (     |'  `. | |    \ .'  _/  (     |,---.  /  `. /   \ |    \".Centered().Cross());
            Console.WriteLine(crosshair.Substring(0,crosshair.Length-1).Red());
            Console.WriteLine(@" \___.' /    | / |`---' `.___, \___.' /    | `.__/| /     |`---'".Centered().Cross());
            Console.WriteLine(@"                 \                                        \     ".Centered().Cross());
            Console.WriteLine("".Centered().Cross());
        }

        public static void Inform(string message){
            Console.WriteLine("[info]".Blue() + $" {message}");
        }

        public static void Warn(string message){
            Console.WriteLine("[warning]".Yellow() + $" {message}");
        }

        public static void Error(string message){
            Console.WriteLine("[error]".Red() + $" {message}");
        }

        public static void Input(string message){
            Console.Write("[input]".Blue() + $" {message}: ");
        }
    }
}