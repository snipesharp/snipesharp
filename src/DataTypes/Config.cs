namespace DataTypes
{
    // should not be edited directly
    // this is a template for the settings and their default values
    // Config.v.something should be used instead!
    // There's an extra ".v." BUT it can be used from anywhere!
    // Also, a property should have { get; set; } for the deserialization of it to work.
    public class ConfigSettings
    {
        public bool debug = false;
        public bool awaitFirstPacket = false;
        public string defaultSkin = "https://raw.githubusercontent.com/snipesharp/snipesharp/main/default_skin.png";
        public bool firstTime = false;
        public bool yggdrasilToken = false;
        public string discordApplicationId = "946380985791033374";
        public string snipesharpServerWebhook = "http://webhooks.snipesharp.xyz:5150";
        public bool EnableDiscordRPC {get;set;} = true;
        public bool ShowUsernameDRPC {get;set;} = true;
        public bool ShowTargetNameDRPC {get;set;} = true;
        public bool SnipesharpServerWebhook { get; set; } = true;
        public string CustomDiscordWebhookUrl { get; set; } = "";
        public string DiscordWebhookUsername { get; set; } = Environment.UserName;
        public bool EnableBearerRefreshing {get;set;} = true;
        public int SendPacketsCount { get; set; } = 3;
        public int PacketSpreadMs { get; set; } = 390;
        public bool AutoSkinChange { get; set; } = false;
        public string SkinUrl { get; set; } = "https://raw.githubusercontent.com/snipesharp/snipesharp/main/default_skin.png";
        public string SkinType { get; set; } = "classic";
        public bool NamesListAutoClean { get; set; } = true;
        public bool AlwaysAutoUpdate {get;set;} = false;
        public bool NeverAutoUpdate {get;set;} = false;
    }

    public static class Config
    {
        public static ConfigSettings v = new ConfigSettings();
        public static void Prepare() {
            if (v.DiscordWebhookUsername.Length > 16) v.DiscordWebhookUsername = v.DiscordWebhookUsername.Substring(0, 16);
            v.CustomDiscordWebhookUrl = v.CustomDiscordWebhookUrl.Replace("http:", "https:");
        }
    }
}
