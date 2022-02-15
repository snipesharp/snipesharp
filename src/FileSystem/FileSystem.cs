using System.Text.Json;
using DataTypes;

namespace FS
{
    public static class FileSystem
    {
        static string snipesharpFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.snipesharp\";
        static string accountJsonFile = snipesharpFolder + "account.json";
        
        // Saves the given string to the account.txt file
        public static void SaveAccount(Account account){
            try {
                var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(accountJsonFile, json);
            } catch (Exception e) { Cli.Output.Error(e.Message); }
        }
    
        public static Account GetAccount() {
            return JsonSerializer.Deserialize<Account>(File.ReadAllText(accountJsonFile));
        }

        /// <returns>true if account.txt exists in the snipesharp folder</returns>
        public static bool AccountFileExists() {
            return File.Exists(accountJsonFile);
        }
    }
}
