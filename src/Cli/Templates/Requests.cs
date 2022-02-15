using DataTypes.SetText;

namespace Cli.Templates
{
    public class Requests
    {
        public static string Bearer = $"Paste your {SetText.Blue}Bearer Token{SetText.ResetAll}: ";
        public static string Email = $"Enter your Mojang account {SetText.Blue}Email{SetText.ResetAll}: ";
        public static string Password = $"Enter your Mojang account {SetText.Blue}Password{SetText.ResetAll}: ";
    }
}