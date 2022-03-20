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
                    // move to /usr/bin/snipesharp
                    File.Move(Process.GetCurrentProcess().MainModule!.FileName!, "/usr/bin/snipesharp");
                }
                catch (UnauthorizedAccessException) {
                    Cli.Output.ExitError($"Failed to install to {DataTypes.SetText.SetText.Blue}/usr/bin/snipesharp{DataTypes.SetText.SetText.ResetAll} due to lack of permissions (Re-run with sudo)");
                }
                catch (Exception e) {
                    Cli.Output.ExitError($"Failed to install: {e.Message}");
                }

                Cli.Output.Success($"Successfully installed snipesharp to {DataTypes.SetText.SetText.Blue}/usr/bin/snipesharp{DataTypes.SetText.SetText.ResetAll}");
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
                File.CreateSymbolicLink(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), snipesharpExe);
            }
            catch (UnauthorizedAccessException) {
                Cli.Output.ExitError($"Failed to install to {DataTypes.SetText.SetText.Blue}%APPDATA%\\.snipesharp{DataTypes.SetText.SetText.ResetAll} due to lack of permissions (Re-run with sudo)");
            }
            catch (Exception e) {
                Cli.Output.ExitError($"Failed to install: {e.Message}");
            }

            Cli.Output.Success($"Successfully installed snipesharp to {DataTypes.SetText.SetText.Blue}{snipesharpExe}{DataTypes.SetText.SetText.ResetAll}");
            Environment.Exit(0);
            return;
        }
    }
}