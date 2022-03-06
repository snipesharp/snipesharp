using System.Text.Json;
using DataTypes;

namespace Utils
{
    public static class AccountSerialization
    {
        public static void Serialize(string path){
            var json = JsonSerializer.Serialize(Account.v, new JsonSerializerOptions { WriteIndented = true });
            var warning = string.Concat(Enumerable.Repeat(Cli.Templates.TFileSystem.FSInforms.AccountSafety, 50));
            File.WriteAllText(path, warning + json);
        }

        public static void Deserialize(string textContent) {
            var account = JsonSerializer.Deserialize<AccountSettings>(textContent);
            Account.v = account!;
        }
    }
}