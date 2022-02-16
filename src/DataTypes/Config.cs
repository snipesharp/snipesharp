namespace DataTypes
{
    public class Config
    {
        public int sendPacketsCount = 3; // anything above 3 results in TooManyPackets
        public string snipesharpServerWebhook = "https://discord.com/api/webhooks/943491871731228742/cgUCq2maxb7cTW-SNCjD-e8wMxJYZB4Dgzb62YNdSjGeQS3dgPbDayUzP4u8S4oJXdc9";
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
