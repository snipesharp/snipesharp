namespace DataTypes
{
    public class Config
    {
        public int sendPacketsCount = 3; // anything above 3 results in TooManyPackets
        public string snipesharpServerWebhook = "https://discord.com/api/webhooks/943491871731228742/cgUCq2maxb7cTW-SNCjD-e8wMxJYZB4Dgzb62YNdSjGeQS3dgPbDayUzP4u8S4oJXdc9";
        public int PacketSpreadMs { get; set; }
        public string WebhookUsername { get; set; }
        public bool SnipesharpServerWebhook { get; set; }
        public string DiscordWebhookUrl { get; set; }
        public string SkinUrl { get; set; }
        public string SkinType { get; set; }
        
        public Config() { 
            PacketSpreadMs = 31;
            WebhookUsername = Environment.UserName;
            SnipesharpServerWebhook = true;
            DiscordWebhookUrl = "";
            SkinUrl = "";
            SkinType = "classic";
        }
        
        public Config Fix() {
            if (WebhookUsername.Length > 16) WebhookUsername = WebhookUsername.Substring(0, 16);
            DiscordWebhookUrl = DiscordWebhookUrl.Replace("http:", "https:");
            return this;
        }
    }
}
