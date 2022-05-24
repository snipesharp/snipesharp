using System.Text.Json.Serialization;

namespace DataTypes
{
    public class AccountSettings
    {
        public string emailInUse = "None";
        public bool prename;
        public string Bearer { get; set; }
        public string MicrosoftEmail { get; set; }
        public string MicrosoftPassword { get; set; }
        public string MojangEmail { get; set; }
        public string MojangPassword { get; set; }
    }
    public static class Account {
        public static AccountSettings v = new AccountSettings();
    }
}
