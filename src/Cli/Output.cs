using Cli.Templates;
using StringExtensions;
using DataTypes.SetText;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            // construct crosshair string
            var crosshair = new string('-', Console.WindowWidth);
            var halfLength = crosshair.Length / 2;
            var even = Convert.ToInt32(Console.WindowWidth % 2 == 0);
            crosshair = $"{crosshair.Substring(0, halfLength)}+{crosshair.Substring(halfLength, halfLength - even)}";

            // account for smaller terminal window
            if (Console.WindowWidth < 65 && Console.WindowWidth > 33)
            {
                Console.WriteLine("".Centered().Cross());
                Console.WriteLine(SetText.LightCyan +   @"   ____ , __   ` \,___,   ___ ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +        @"  (     |'  `. | |    \ .'___/".Centered().Cross());
                Console.WriteLine(SetText.Blue +        @"   `--. |    | | |    | |     ".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue +    @" \___.' /    | / |`---' `.___,".Centered().Cross());
                Console.WriteLine(SetText.DarkRed + crosshair);
                Console.WriteLine(SetText.LightCyan +   @"   ____ /        __   .___  \,___,".Centered().Cross());
                Console.WriteLine(SetText.Cyan +        @"  (     |,---.  /  `. /   \ |    \".Centered().Cross());
                Console.WriteLine(SetText.Blue +        @"   `--. |    | |    | |     |    |".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue +    @" \___.' /    | `.__/| /     |`---'".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue +    @"                            \     ".Centered().Cross());
                Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}".Centered());
                return;
            }
            if (Console.WindowWidth < 33){
                Console.WriteLine($"{SetText.Blue}snipe{SetText.White}sharp".Centered());
                Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}" + SetText.ResetAll.Centered());
                return;
            }

            // print
            Console.WriteLine("".Centered().Cross());
            Console.WriteLine(SetText.LightCyan +@"   ____ , __   ` \,___,   ___    ____ /        __   .___  \,___,".Centered().Cross());
            Console.WriteLine(SetText.Cyan      +@"  (     |'  `. | |    \ .'  _/  (     |,---.  /  `. /   \ |    \".Centered().Cross());
            Console.WriteLine(SetText.DarkRed   +crosshair);
            Console.WriteLine(SetText.Blue      +@" \___.' /    | / |`---' `.___, \___.' /    | `.__/| /     |`---'".Centered().Cross());
            Console.WriteLine(SetText.DarkBlue  +@"                 \                                        \     ".Centered().Cross());
            Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}".Centered());
        }

        public static void Inform(string message){
            FS.FileSystem.Log($"Info: {message}");
            Console.WriteLine($"   {SetText.Gray}i{SetText.ResetAll} {message}{SetText.ResetAll}");
        }

        public static void Warn(string message){
            FS.FileSystem.Log($"Warning: {message}");
            Console.WriteLine($"   {SetText.Yellow}!{SetText.ResetAll} {message}{SetText.ResetAll}");
        }

        public static void Error(string message){
            FS.FileSystem.Log($"Error: {message}");
            Console.WriteLine($"   {SetText.Red}x{SetText.ResetAll} {message}{SetText.ResetAll}");
        }
        public static void Success(string message){
            FS.FileSystem.Log($"Success: {message}");
            Console.WriteLine($"   {SetText.Green}✓{SetText.ResetAll} {message}{SetText.ResetAll}");
        }

        public static void ExitError(string message){
            Error(message);
            Utils.DiscordRPC.Deinitialize();
            new Animatables.RainbowText(TFunnyErrors.GetRandom());
            Console.ReadKey();
            
            SetText.DisplayCursor(true);
            Environment.Exit(1);
        }

        public static void Input(string message){
            Console.Write($"{SetText.DarkBlue}[{SetText.Blue}input{SetText.DarkBlue}]{SetText.ResetAll} {message}{SetText.ResetAll}");
        }
    }
}