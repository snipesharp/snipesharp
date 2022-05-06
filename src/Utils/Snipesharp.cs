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