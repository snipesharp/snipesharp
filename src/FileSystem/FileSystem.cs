using System.Text.Json;
using DataTypes;

namespace FS
{
    public static class FileSystem
    {
        static string snipesharpFolder = Cli.Core.pid != PlatformID.Unix 
            ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.snipesharp\" 
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/.snipesharp/";
        static string logsFolder = Cli.Core.pid != PlatformID.Unix
            ? snipesharpFolder + @"\logs\"
            : snipesharpFolder + @"/logs/";
        static string accountJsonFile = snipesharpFolder + "account.json";
        static string configJsonFile = snipesharpFolder + "config.json";
        static string namesJsonFile = snipesharpFolder + "names.json";
        static string logFile = logsFolder + @"latest.log";

        /// <summary>
        /// Creates the .snipesharp folder and informs the user about it
        /// </summary>
        public static void CreateSnipesharpFolder()
        {
            Directory.CreateDirectory(snipesharpFolder);
            Cli.Output.Inform($"All config files can be found in \"{DataTypes.SetText.SetText.Blue}{snipesharpFolder}{DataTypes.SetText.SetText.White}\"");
        }
        /// <summary>
        /// Saves given names object to the names.json file
        /// </summary>
        public static void SaveNames(List<string> names, string? path=null)
        {
            path = path == null ? namesJsonFile : snipesharpFolder + path;
            try
            {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                var json = JsonSerializer.Serialize(names, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(path, json);
            }
            catch (Exception e) { Cli.Output.Error(e.Message); }
        }
        /// <summary>
        /// Saves given account to the account.json file
        /// </summary>
        public static void SaveAccount(Account account){
            try {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(accountJsonFile, json);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }

        /// <summary>
        /// Saves given config to the config.json file
        /// </summary>
        public static void SaveConfig(Config config){
            try {
                if (!Directory.Exists(snipesharpFolder)) CreateSnipesharpFolder();
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configJsonFile, json);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }

        /// <returns>a string array of names in the names.json file</returns>
        public static List<string> GetNames()
        {
            if (!NamesFileExists()) return new List<string>();
            return JsonSerializer.Deserialize<List<string>>(File.ReadAllText(namesJsonFile));
        }

        /// <returns>an existing or new config depending on whether one already exists</returns>
        public static Config GetConfig() {
            if (!ConfigFileExists()) return new Config();
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(configJsonFile));
        }

        /// <returns>an existing or new account config depending on whether one already exists</returns>
        public static Account GetAccount() {
            if (!AccountFileExists()) return new Account();
            return JsonSerializer.Deserialize<Account>(File.ReadAllText(accountJsonFile));
        }

        /// <summary>
        /// Checks whether the names.json file exists in the snipesharp folder
        /// </summary>
        public static bool NamesFileExists()
        {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(namesJsonFile);
        }

        /// <summary>
        /// Checks whether the account.json file exists in the snipesharp folder
        /// </summary>
        public static bool AccountFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(accountJsonFile);
        }

        /// <summary>
        /// Checks whether the config.json file exists in the snipesharp folder
        /// </summary>
        public static bool ConfigFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(configJsonFile);
        }
        /// <summary>
        /// Checks whether the latest.log file exists in the snipesharp folder
        /// </summary>
        public static bool LogFileExists()
        {
            if (!Directory.Exists(logsFolder)) return false;
            return File.Exists(logFile);
        }

        /// <summary>
        /// appends the given string to the latest.log file
        /// </summary>
        public static void Log(string toLog)
        {
            if (!Directory.Exists(logsFolder)) Directory.CreateDirectory(logsFolder);
            File.AppendAllText(logFile, $"[{DateTime.Now}] {toLog}");
        }
        /// <returns>path to latest.log file</returns>
        public static string GetLatestLogPath()
        {
            return logFile;
        }
    }
}
