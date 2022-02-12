using StringExtensions;
using DataTypes.SetText;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            // construct crosshair string
            var crosshair = new string('-', 
                (Console.WindowWidth % 2 == 0)
                ? Console.WindowWidth
                : Console.WindowWidth - 1);
            var halfLength = crosshair.Length / 2;
            crosshair = $"{crosshair.Substring(0, halfLength)}+{crosshair.Substring(halfLength, halfLength)}";

            // print
            Console.WriteLine("".Centered().Cross());
            Console.WriteLine(SetText.LightCyan +@"   ____ , __   ` \,___,   ___    ____ /        __   .___  \,___,".Centered().Cross());
            Console.WriteLine(SetText.Cyan +     @"  (     |'  `. | |    \ .'  _/  (     |,---.  /  `. /   \ |    \".Centered().Cross());
            Console.WriteLine(SetText.DarkRed + crosshair);
            Console.WriteLine(SetText.Blue +     @" \___.' /    | / |`---' `.___, \___.' /    | `.__/| /     |`---'".Centered().Cross());
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