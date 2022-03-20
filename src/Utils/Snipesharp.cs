using System.Diagnostics;

namespace Utils
{
    public class Snipesharp
    {
        public string GetAssemblyVersion() {
            string fullVersion = GetType().Assembly.GetName().Version.ToString();
            return fullVersion.Substring(0, fullVersion.Length - 2);
        }

        public static void Install() {
            if (Cli.Core.pid == PlatformID.Unix) {
                // install for unix
                try {
                    // output info because after file is moved it cant output
                    Cli.Output.Inform($"Installing snipesharp to {DataTypes.SetText.SetText.Blue}/usr/bin/snipesharp{DataTypes.SetText.SetText.ResetAll}");

                    // move to /usr/bin/snipesharp
                    File.Move(Process.GetCurrentProcess().MainModule!.FileName!, "/usr/bin/snipesharp");
                }
                catch (UnauthorizedAccessException) {
                    Cli.Output.Error($"Failed to install to {DataTypes.SetText.SetText.Blue}/usr/bin/snipesharp{DataTypes.SetText.SetText.ResetAll} due to lack of permissions (Re-run with sudo)");
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
            var snipesharpExe = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.snipesharp\\snipesharp.exe";

            // install on windows
            try {

                // move file to .snipesharp folder
                File.Move(Process.GetCurrentProcess().MainModule!.FileName!, snipesharpExe);

                // create desktop shortcut
                try {
                    File.CreateSymbolicLink(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//snipesharp.lnk", snipesharpExe);
                }
                catch {
                    Cli.Output.Error($"Failed to create desktop shortcut");
                }
            }
            catch (UnauthorizedAccessException uae) {
                Cli.Output.Error($"Failed to install to {DataTypes.SetText.SetText.Blue}%APPDATA%\\.snipesharp{DataTypes.SetText.SetText.ResetAll} due to lack of permissions ({uae.Message})");
                Environment.Exit(-1);
            }
            catch (Exception e) {
                Cli.Output.Error($"Failed to install: {e.Message}");
                Environment.Exit(-1);
            }

            Cli.Output.Success($"Successfully installed snipesharp to {DataTypes.SetText.SetText.Blue}{snipesharpExe}{DataTypes.SetText.SetText.ResetAll}");
            Environment.Exit(0);
            return;
        }
    }
}