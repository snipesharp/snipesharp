using DataTypes;

namespace ConfigExtensions
{
    public static class ConfigExtension
    {
        public static Config Fix(this Config config)
        {
            if (config.WebhookUsername.Length > 16) config.WebhookUsername = config.WebhookUsername.Substring(0, 16);
            config.DiscordWebhookLink = config.DiscordWebhookLink.Replace("http:", "https:");
            return config;
        }
    }
}
