using StringExtensions;
using DataTypes.SetText;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            // account for smaller terminal window
            if(Console.WindowWidth < 65){
                Console.WriteLine($"{SetText.Blue}snipe{SetText.White}sharp".Centered());
                return;
            }

            // construct crosshair string
            var crosshair = new string('-', Console.WindowWidth);
            var halfLength = crosshair.Length / 2;
            var even = Convert.ToInt32(Console.WindowWidth % 2 == 0);
            crosshair = $"{crosshair.Substring(0, halfLength)}+{crosshair.Substring(halfLength, halfLength - even)}";

            // print
            Console.WriteLine("".Centered().Cross());
            Console.WriteLine(SetText.LightCyan +@"   ____ , __   ` \,___,   ___    ____ /        __   .___  \,___,".Centered().Cross());
            Console.WriteLine(SetText.Cyan      +@"  (     |'  `. | |    \ .'  _/  (     |,---.  /  `. /   \ |    \".Centered().Cross());
            Console.WriteLine(SetText.DarkRed   +crosshair);
            Console.WriteLine(SetText.Blue      +@" \___.' /    | / |`---' `.___, \___.' /    | `.__/| /     |`---'".Centered().Cross());
            Console.WriteLine(SetText.DarkBlue  +@"                 \                                        \     ".Centered().Cross());
            Console.WriteLine("".Centered().Cross());
        }

        public static void Inform(string message){
            Console.WriteLine($"{SetText.Gray}[{SetText.White}info{SetText.Gray}]{SetText.ResetAll} {message}");
        }

        public static void Warn(string message){
            Console.WriteLine($"{SetText.Yellow}[warning]{SetText.ResetAll} {message}");
        }

        public static void Error(string message){
            Console.WriteLine($"{SetText.Red}[error]{SetText.ResetAll} {message}");
        }
        public static void Success(string message)
        {
            Console.WriteLine($"{SetText.DarkGreen}[{SetText.Green}success{SetText.DarkGreen}]{SetText.ResetAll} {message}");
        }

        public static void ExitError(string message){
            Error(message);
            Environment.Exit(1);
        }

        public static void Input(string message){
            Console.Write($"{SetText.DarkBlue}[{SetText.Blue}input{SetText.DarkBlue}]{SetText.ResetAll} {message}");
        }
    }
}