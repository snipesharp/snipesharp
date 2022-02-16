namespace DataTypes
{
    public class Config
    {
        public int sendPacketsCount = 3; // anything above 3 results in TooManyPackets
        public int PacketSpreadMs { get; set; }
        public string WebhookUsername { get; set; }
        public bool SnipesharpServerWebhook { get; set; }
        public string DiscordWebhookLink { get; set; }
        public Config()
        { 
            PacketSpreadMs = 31;
            WebhookUsername = Environment.UserName;
            SnipesharpServerWebhook = true;
            DiscordWebhookLink = "";
        }
    }
}
