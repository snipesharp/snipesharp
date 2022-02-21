namespace DataTypes
{
    public class Config
    {
        public int sendPacketsCount = 3; // anything above 3 results in TooManyPackets
        public string snipesharpServerWebhook = "https://discord.com/api/webhooks/943491871731228742/cgUCq2maxb7cTW-SNCjD-e8wMxJYZB4Dgzb62YNdSjGeQS3dgPbDayUzP4u8S4oJXdc9";
        public int PacketSpreadMs { get; set; } = 31;
        public string WebhookUsername { get; set; } = Environment.UserName;
        public bool SnipesharpServerWebhook { get; set; } = true;
        public string DiscordWebhookUrl { get; set; } = "";
        public bool AutoSkinChange { get; set; } = false;
        public string SkinUrl { get; set; } = "https://raw.githubusercontent.com/snipesharp/snipesharp/main/default_skin.png";
        public string SkinType { get; set; } = "classic";
        public bool NamesListAutoClean { get; set; } = true;
        public bool RefreshBearer { get; set; } = true;
        
        public Config Fix() {
            if (WebhookUsername.Length > 16) WebhookUsername = WebhookUsername.Substring(0, 16);
            DiscordWebhookUrl = DiscordWebhookUrl.Replace("http:", "https:");
            return this;
        }
    }
}
