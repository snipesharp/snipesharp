using System.Text.Json;
using DataTypes;

namespace FS
{
    public static class FileSystem
    {
        static string snipesharpFolder = Cli.Core.pid != PlatformID.Unix 
            ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.snipesharp\" 
            : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"/.snipesharp/";
        static string accountJsonFile = snipesharpFolder + "account.json";
        static string configJsonFile = snipesharpFolder + "config.json";

        /// <summary>
        /// Saves given account to the account.json file
        /// </summary>
        /// <param name="account"></param>
        public static void SaveAccount(Account account){
            try {
                if (!Directory.Exists(snipesharpFolder)) Directory.CreateDirectory(snipesharpFolder);
                var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(accountJsonFile, json);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }
        
        public static void SaveConfig(Config config){
            try {
                if (!Directory.Exists(snipesharpFolder)) Directory.CreateDirectory(snipesharpFolder);
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(configJsonFile, json);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }
        
        public static Config GetConfig() {
            if (!ConfigFileExists()) return new Config();
            return JsonSerializer.Deserialize<Config>(File.ReadAllText(configJsonFile));
        }

        public static Account GetAccount() {
            if (!AccountFileExists()) return new Account();
            return JsonSerializer.Deserialize<Account>(File.ReadAllText(accountJsonFile));
        }

        // Checks whether the account.json exists in the snipesharp folder
        public static bool AccountFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(accountJsonFile);
        }
        
        // Checks whether the config.json exists in the snipesharp folder
        private static bool ConfigFileExists() {
            if (!Directory.Exists(snipesharpFolder)) return false;
            return File.Exists(configJsonFile);
        }
    }
}
