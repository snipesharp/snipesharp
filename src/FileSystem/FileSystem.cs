using System.Text.Json;
using DataTypes;

namespace FS
{
    public static class FileSystem
    {
        static string snipesharpFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\.snipesharp\";
        static string accountJsonFile = snipesharpFolder + "account.json";
        /// <summary>
        /// Saves the given string to the account.txt file
        /// </summary>
        /// <param name="path">Path to save to</param>
        /// <param name="toSave">String to save</param>
        public static void SaveAccount(Account account)
        {
            try
            {
                var json = JsonSerializer.Serialize(account, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(accountJsonFile, json);
            }
            catch (Exception ex)
            {
                Cli.Output.Error(ex.Message);
            }
        }
        /// <returns>an Account object with contents of the account.json file</returns>
        public static Account GetAccount()
        {
            return JsonSerializer.Deserialize<Account>(File.ReadAllText(accountJsonFile));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if account.txt exists in the snipesharp folder</returns>
        public static bool AccountFileExists()
        {
            return File.Exists(accountJsonFile);
        }
    }
}
