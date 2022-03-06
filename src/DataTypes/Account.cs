using System.Text.Json.Serialization;

namespace DataTypes
{
    public class AccountSettings
    {
        public static bool prename;
        public static string Bearer { get; set; }
        public static string MicrosoftEmail { get; set; }
        public static string MicrosoftPassword { get; set; }
        [JsonPropertyName("sq1")]
        public static string SecurityQuestion1 { get; set; }
        [JsonPropertyName("sq2")]
        public static string SecurityQuestion2 { get; set; }
        [JsonPropertyName("sq3")]
        public static string SecurityQuestion3 { get; set; }
    }
    public static class Account {
        public static AccountSettings v = new AccountSettings();
    }
}
