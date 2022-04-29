using System.Diagnostics;
using DataTypes.SetText;
using StringExtensions;

namespace Utils
{
    public class Snipesharp
    {
        public static DateTime snipeTime = new DateTime();
        public string GetAssemblyVersion() {
            string fullVersion = GetType().Assembly.GetName().Version.ToString();
            return fullVersion.Substring(0, fullVersion.Length - 2);
        }
        public static string GetNameToSnipe() {
            if (!Cli.Core.arguments.ContainsKey("--name")) {
                List<string> argNamesList = FS.FileSystem.GetNames();
                return new Cli.Animatables.SelectionPrompt("What name(s) would you like to snipe?",
                    new string[] {
                        Cli.Templates.TNames.LetMePick,
                        Cli.Templates.TNames.UseNamesJson,
                        Cli.Templates.TNames.ThreeCharNames,
                    },
                    new string[] {
                        argNamesList.Count == 0 ? Cli.Templates.TNames.UseNamesJson : "",
                    }
                ).result;
            }
            return Cli.Core.arguments["--name"].data!;
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

        public static void Install() {
            if (Cli.Core.pid == PlatformID.Unix) {
                // install for unix
                try {
                    // output info because after file is moved it cant output
                    Cli.Output.Inform($"Installing snipesharp to {SetText.Blue}/usr/bin/snipesharp{SetText.ResetAll}");

                    // move to /usr/bin/snipesharp
                    File.Move(Process.GetCurrentProcess().MainModule!.FileName!, "/usr/bin/snipesharp", true);

                    // output success
                    Cli.Output.Success($"Successfully installed snipesharp to {SetText.Blue}/usr/bin/snipesharp{SetText.ResetAll}");
                }
                catch (UnauthorizedAccessException) {
                    Cli.Output.Error($"Failed to install to {SetText.Blue}/usr/bin/snipesharp{SetText.ResetAll} due to lack of permissions (Re-run with sudo)");
                    Environment.Exit(-1);
                }
                catch (Exception e) {
                    Cli.Output.Error($"Failed to install: {e.Message}");
                    Environment.Exit(-1);
                }

                Environment.Exit(0);
                return;
            }
            
            // exe location
            var snipesharpExe = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\.snipesharp\\snipesharp.exe";

            // install on windows
            try {
                string SCRIPT = $"{Environment.GetFolderPath(Environment.SpecialFolder.Templates)}\\snipesharp_install.vbs";
                string makeShortcutCommand =    "echo Creating desktop shortcut for snipesharp.exe && " +
                                                $"echo Creating script at {SCRIPT} && " +
                                                $"echo Set oWS = WScript.CreateObject(\"WScript.Shell\") >> {SCRIPT} && " +
                                                $"echo sLinkFile = \"%USERPROFILE%\\Desktop\\snipesharp.lnk\" >> {SCRIPT} && " +
                                                $"echo Set oLink = oWS.CreateShortcut(sLinkFile) >> {SCRIPT} && " +
                                                $"echo oLink.TargetPath = \"%AppData%\\.snipesharp\\snipesharp.exe\" >> {SCRIPT} && " +
                                                $"echo oLink.WorkingDirectory = \"%AppData%\\.snipesharp\" >> {SCRIPT} && " +
                                                $"echo oLink.Save >> {SCRIPT} && " +
                                                $"cscript /nologo {SCRIPT} && " +
                                                $"del {SCRIPT}";
                // move file to .snipesharp folder
                File.Move(Process.GetCurrentProcess().MainModule!.FileName!, snipesharpExe, true);

                // create desktop shortcut
                try {
                    Process.Start(new ProcessStartInfo {
                        FileName = "cmd.exe",
                        Arguments = $"/C {makeShortcutCommand}"
                    });

                    Cli.Output.Success($"Successfully created Desktop shortcut");
                }
                catch {
                    Cli.Output.Error($"Failed to create desktop shortcut");
                }
            }
            catch (UnauthorizedAccessException uae) {
                Cli.Output.Error($"Failed to install to {SetText.Blue}%APPDATA%\\.snipesharp{SetText.ResetAll} due to lack of permissions ({uae.Message})");
                Environment.Exit(-1);
            }
            catch (Exception e) {
                Cli.Output.Error($"Failed to install: {e.Message}");
                Environment.Exit(-1);
            }

            Cli.Output.Success($"Successfully installed snipesharp to {SetText.Blue}{snipesharpExe}{SetText.ResetAll}");
            Environment.Exit(0);
            return;
        }
    }
}