using Cli.Templates;
using StringExtensions;
using DataTypes.SetText;
using System.Drawing;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            // construct crosshair string
            var crosshair = new string('-', Console.WindowWidth);
            var halfLength = crosshair.Length / 2;
            var even = Convert.ToInt32(Console.WindowWidth % 2 == 0);
            crosshair = $"{crosshair.Substring(0, halfLength)}+{crosshair.Substring(halfLength, halfLength - even)}";

            // HUGE
            if (Console.WindowWidth > 123) {
                Console.WriteLine("".Centered().Cross());
                
                Console.WriteLine(SetText.LightCyan +@"   .x+=:.                    .                                .x+=:.                                                      ".Centered().Cross());
                Console.WriteLine(SetText.LightCyan +@"  z`    ^%                  @88>                             z`    ^%    .uef^'                                           ".Centered().Cross());
                Console.WriteLine(SetText.LightCyan +@"     .   <k    u.    u.     %8P    .d``                         .   <k :d88E                      .u    .    .d``         ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +     @"   .@8Ned8'  x@88k u@88c.    .     @8Ne.   .u        .u       .@8Ned8' `888E             u      .d88B :@8c   @8Ne.   .u   ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +     @" .@^%8888'  ^'8888''8888'  .@88u   %8888:u@88N    ud8888.   .@^%8888'   888E .z8k     us888u.  ='8888f8888r  %8888:u@88N  ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +     @"x88:  `)8b.   8888  888R  ''888E`   `888I  888. :888'8888. x88:  `)8b.  888E~?888L .@88 '8888'   4888>'88'    `888I  888. ".Centered().Cross());
                Console.WriteLine(SetText.Blue +     @"8888N=*8888   8888  888R    888E     888I  888I d888 '88%' 8888N=*8888  888E  888E 9888  9888    4888> '       888I  888I ".Centered().Cross());
                Console.WriteLine(SetText.DarkRed + crosshair);
                Console.WriteLine(SetText.Blue +     @"  @8Wou 9%    8888  888R    888E   uW888L  888' 8888L        @8Wou 9%   888E  888E 9888  9888   .d888L .+    uW888L  888' ".Centered().Cross());
                Console.WriteLine(SetText.Blue +     @".888888P`    '*88*' 8888'   888&  '*88888Nu88P  '8888c. .+ .888888P`    888E  888E 9888  9888   ^'8888*'    '*88888Nu88P  ".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue + @"`   ^'F        ''   'Y'     R888' ~ '88888F`     '88888%   `   ^'F     m888N= 888> '888*''888'     'Y'      ~ '88888F`    ".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue + @"                             ''      888 ^         'YP'                 `Y'   888   ^Y'   ^Y'                  888 ^      ".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue + @"                                     *8E                                     J88'                              *8E        ".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue + @"                                     '8>                                     @%                                '8>        ".Centered().Cross());

                Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}".Centered());
                return;
            }

            // SMALL
            if (Console.WindowWidth < 54 && Console.WindowWidth > 30)
            {
                Console.WriteLine("".Centered().Cross());
                Console.WriteLine(SetText.LightCyan +   @"   _________  ______  ___ ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +        @"  / ___/ __ \/ / __ \/ _ \".Centered().Cross());
                Console.WriteLine(SetText.Blue +        @" (__  / / / / / /_/ /  __|".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue +    @"/____/_/ /_/_/ .___/\___/ ".Centered().Cross());
                Console.WriteLine(SetText.DarkRed + crosshair);
                Console.WriteLine(SetText.LightCyan +   @"   _____/ /_  ____ __________ ".Centered().Cross());
                Console.WriteLine(SetText.Cyan +        @"  / ___/ __ \/ __ `/ ___/ __ \".Centered().Cross());
                Console.WriteLine(SetText.Blue +        @" (__  / / / / /_/ / /  / /_/ /".Centered().Cross());
                Console.WriteLine(SetText.DarkBlue +    @"/____/_/ /_/\__,_/_/  / .___/ ".Centered().Cross());
                Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}".Centered());
                return;
            }

            // TINY
            if (Console.WindowWidth < 30){
                Console.WriteLine($"{SetText.Blue}snipe{SetText.White}sharp".Centered());
                Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}" + SetText.ResetAll.Centered());
                return;
            }

            // NORMAL
            Console.WriteLine(SetText.LightCyan +@"                                 __                   ".Centered().Cross());
            Console.WriteLine(SetText.Cyan      +@"   _________  ______  ___  _____/ /_  ____ __________ ".Centered().Cross());
            Console.WriteLine(SetText.Blue      +@"  / ___/ __ \/ / __ \/ _ \/ ___/ __ \/ __ `/ ___/ __ \".Centered().Cross());
            Console.WriteLine(SetText.DarkRed   +crosshair);
            Console.WriteLine(SetText.Blue  +@"/____/_/ /_/ / .___/\___/____/_/ /_/\__,_/_/  / .___/ ".Centered().Cross());
            Console.WriteLine(SetText.DarkBlue  +@"            /_/                              /_/      ".Centered().Cross());
            Console.WriteLine($"{SetText.Blue}https://snipe{SetText.White}sharp.xyz{SetText.ResetAll}".Centered());
        }
        public static void PrintHelp() {
            Console.Write(
                "\n" +
                "Running with arguments is completely optional, none of the available arguments are essential to using snipesharp!\n\n" +
                "Help for using arguments in snipesharp:".Centered() +
                $"\n\nTo give value to an argument; put an {SetText.Blue}equals sign{SetText.ResetAll} after it, followed by an appropriate value\n" +
                $"Example: --spread{SetText.Blue}=400{SetText.ResetAll}\n\n" +
                "Some arguments may not require values\n" +
                $"Example: snipesharp {SetText.Blue}--asc{SetText.ResetAll}\n\n" +
                $"Using arguments colored {SetText.Red}RED{SetText.ResetAll} could result in unwanted outcomes. Only use them if you know what you're doing!\n\n" +
                "Available arguments:".Centered() +
                "\n\n" +
                "--" + SetText.Cyan + "help".MakeGapRight(23) + SetText.ResetAll + $"Prints help for using arguments\n" +
                "--" + SetText.Cyan + "install".MakeGapRight(23) + SetText.ResetAll + $"Installs snipesharp in {SetText.Blue}"
                    + (Cli.Core.pid == PlatformID.Unix
                    ? $"/usr/bin/snipesharp{SetText.ResetAll} ({SetText.Blue}Needs to be ran as superuser{SetText.ResetAll})\n"
                    : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\.snipesharp\\snipesharp.exe{SetText.ResetAll}\n") +
                "--" + SetText.Cyan + "disable-auto-update".MakeGapRight(23) + SetText.ResetAll + $"Prevents snipesharp from checking for software updates\n" +
                "--" + SetText.Cyan + "disable-discordrpc".MakeGapRight(23) + SetText.ResetAll + $"Disables Discord Rich Presence\n" +
                "--" + SetText.Cyan + "enable-discordrpc".MakeGapRight(23) + SetText.ResetAll + $"Enables Discord Rich Presence\n" +
                "--" + SetText.Cyan + "asc".MakeGapRight(23) + SetText.ResetAll + $"Enables Auto Skin Change\n" +
                "--" + SetText.Cyan + "asc-url".MakeGapRight(23) + SetText.ResetAll + $"Sets the Skin URL for Auto Skin Change ({SetText.Blue}Requires string value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "webhook-url".MakeGapRight(23) + SetText.ResetAll + $"Sets the Custom webhook URL ({SetText.Blue}Requires string value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "offset".MakeGapRight(23) + SetText.ResetAll + $"Sets the offset in milliseconds. Use value 'auto' or 'suggested' to use the suggested value ({SetText.Blue}Requires integer value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "spread".MakeGapRight(23) + SetText.ResetAll + $"Sets the PacketSpreadMs config value ({SetText.Blue}Requires integer value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "packet-count".MakeGapRight(23) + SetText.ResetAll + $"Sets the SendPacketsCount config value ({SetText.Blue}Requires integer value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "prename".MakeGapRight(23) + SetText.ResetAll + $"Automatically sets SendPacketsCount to 2\n" +
                "--" + SetText.Cyan + "username".MakeGapRight(23) + SetText.ResetAll + $"Sets your display name in Discord Rich Presence, if it's enabled ({SetText.Blue}Requires string value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "bearer".MakeGapRight(23) + SetText.ResetAll + $"Sets the Bearer Token account value ({SetText.Blue}Requires string value{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "email".MakeGapRight(23) + SetText.ResetAll + $"Sets the Microsoft login Email ({SetText.Blue}Requires string value{SetText.ResetAll}) ({SetText.Blue}Requires valid --password value to work{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "password".MakeGapRight(23) + SetText.ResetAll + $"Sets the Microsoft login Password ({SetText.Blue}Requires string value{SetText.ResetAll}) ({SetText.Blue}Requires valid --email value to work{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "mojang-email".MakeGapRight(23) + SetText.ResetAll + $"Sets the Mojang login Email ({SetText.Blue}Requires string value{SetText.ResetAll}) ({SetText.Blue}Requires valid --mojang-password value to work{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "mojang-password".MakeGapRight(23) + SetText.ResetAll + $"Sets the Mojang login Password ({SetText.Blue}Requires string value{SetText.ResetAll}) ({SetText.Blue}Requires valid --mojang-email value to work{SetText.ResetAll})\n" +
                "--" + SetText.Cyan + "skip-gc-redeem".MakeGapRight(23) + SetText.ResetAll + $"Skips the giftcard redeeming process\n" +
                "--" + SetText.Red + "test-rl".MakeGapRight(23) + SetText.ResetAll + $"Instantly sends name change packets for the name 'abc' to test rate limiting. Works well combined with --packet-count\n" +
                "--" + SetText.Red + "await-first-packet".MakeGapRight(23) + SetText.ResetAll + $"Sends the second name change packet after a response is received from the first one\n" +
                "--" + SetText.Red + "await-packets".MakeGapRight(23) + SetText.ResetAll + $"Sends every name change packet after a response is received from the one prior to it\n" +
                "--" + SetText.Red + "dont-verify".MakeGapRight(23) + SetText.ResetAll + $"Doesn't verify your Bearer Token works\n"
            );
            Environment.Exit(0);
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