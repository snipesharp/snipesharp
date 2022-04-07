using DataTypes.SetText;

namespace Cli.Templates
{
    public class TRequests
    {
        public static string Bearer = $"Paste your {SetText.Blue}Bearer Token{SetText.ResetAll}: ";
        public static string MicrosoftEmail = $"Enter your Microsoft account {SetText.Blue}Email{SetText.ResetAll}: ";
        public static string MicrosoftPassword = $"Enter your Microsoft account {SetText.Blue}Password{SetText.ResetAll}: ";
        public static string MojangEmail = $"Enter your Mojang account {SetText.Blue}Email{SetText.ResetAll}: ";
        public static string MojangPassword = $"Enter your Mojang account {SetText.Blue}Password{SetText.ResetAll}: ";
        public static string Giftcode = $"Your account doesn't own a copy of Minecraft, {SetText.Blue}redeem a giftcard{SetText.ResetAll}: ";
    }
}