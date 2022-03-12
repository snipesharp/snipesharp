using System.Text.Json;
using DataTypes;
using Cli.Templates;

namespace FS
{
    public static class FileSystem
    {
        // folders
        public static string snipesharpFolder = Cli.Core.pid != PlatformID.Unix 
            ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.snipesharp\" 
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/.snipesharp/";
        public static string logsFolder = Cli.Core.pid != PlatformID.Unix
            ? snipesharpFolder + @"\logs\"
            : snipesharpFolder + @"/logs/";
        
        // files
        public static string accountJsonFile = snipesharpFolder + "account.json";
        public static string configJsonFile = snipesharpFolder + "config.json";
        public static string namesJsonFile = snipesharpFolder + "names.json";
        public static string logFile = logsFolder + $"{DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss")}.log";
        public static string latestLogFile = logsFolder + "latest.log";

        // Creates the .snipesharp folder and informs the user
        public static void CreateSnipesharpFolder() {
            Directory.CreateDirectory(snipesharpFolder);
            Config.v.firstTime = true;
            Cli.Output.Inform(TFileSystem.FSInforms.ConfigSetup);
        }

        // Saves given names list to the names.json file
        public static void SaveNames(List<string> names, string? path = null) {
            path = path == null ? namesJsonFile : snipesharpFolder + path;
            try {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                var json = JsonSerializer.Serialize(names, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception e) { Cli.Output.Error(e.Message); }
        }

        // Saves given account to the account.json file
        public static void UpdateAccount(){
            try {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                Utils.AccountSerialization.Serialize(accountJsonFile);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }

        // Saves the state of the config to the config file
        public static void UpdateConfig(){
            try {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                Utils.ConfigSerialization.Serialize(configJsonFile);
            } catch (Exception e) { Cli.Output.ExitError(e.Message); }
        }

        /// <returns>List of names in the names.json file</returns>
        public static List<string> GetNames() {
            try {
                if (!NamesFileExists()) return new List<string>();
                return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(namesJsonFile))!;
            }
            catch (JsonException e) {
                Cli.Output.Error(TFileSystem.FSInforms.CannotReadFile(new Tuple<string, JsonException>("names.json", e)));
                return new List<string>();
            }
        }

        // used to prepare the config before using it
        public static void PrepareConfig() {
            try {
                if (!ConfigFileExists()) {
                    UpdateConfig();
                    return;
                }
                Utils.ConfigSerialization.Deserialize(File.ReadAllText(configJsonFile));
                Config.Prepare();
                UpdateConfig();
            }
            catch (JsonException e) {
                Cli.Output.Error(TFileSystem.FSInforms.CannotReadFile(new Tuple<string, JsonException>("config.json", e)));
            }
            catch (Exception) {
                Cli.Output.Error("An error occured while reading config.json");
            }
        }

        /// <summary>Prepares existing or new account config depending on whether one already exists</summary>
        public static void PrepareAccount() {
            try {
                if (!AccountFileExists()) {
                    return;
                }
                var fileContents = File.ReadAllText(accountJsonFile);
                var splitted = fileContents.Split(new[] { '{' }, 2);
                Utils.AccountSerialization.Deserialize("{"+splitted[1]);
                UpdateAccount();
            }
            catch (JsonException e) {
                Cli.Output.Error(TFileSystem.FSInforms.CannotReadFile(new Tuple<string, JsonException>("account.json", e)));
            }
            catch (Exception) {
                Cli.Output.Error("An error occured while reading account.json");
            }
        }

        // Checks whether the names.json file exists in the snipesharp folder
        public static bool NamesFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(namesJsonFile);
        }

        // Checks whether the account.json file exists in the snipesharp folder
        public static bool AccountFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(accountJsonFile);
        }

        // Checks whether the config.json file exists in the snipesharp folder
        public static bool ConfigFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(configJsonFile);
        }

        // appends the given string to the latest.log file
        public static void Log(string log) {
            Task.Run(() => {
                try {
                    if (!Directory.Exists(logsFolder)) Directory.CreateDirectory(logsFolder);
                    File.AppendAllText(logFile, $"[{DateTime.Now}] {log}\n");
                    File.AppendAllText(latestLogFile, $"[{DateTime.Now}] {log}\n");
                }
                catch { Cli.Output.Warn("Log file is busy"); }
            });
        }
    }
}
