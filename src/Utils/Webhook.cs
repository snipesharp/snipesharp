using System.Text.Json;

namespace Utils
{
    public class Webhook
    {
        public static void Send(string webhookLink, string content) {
            try {
                HttpClient client = new HttpClient();
                StringContent stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                var response = client.PostAsync(webhookLink, stringContent);
            }
            catch (Exception ex) { Cli.Output.Error(ex.ToString()); }
        }
        
        public static void SendDiscordWebhooks(DataTypes.Config config, string sniped) {
            string json = JsonSerializer.Serialize(new
            { embeds = new[] { new {
                description = $"`{config.WebhookUsername}` sniped [{sniped}](https://namemc.com/{sniped})",
                title = "snipesharp snipe :tada:",
                color = 1211647,
                image = new { url = $"https://mc-heads.net/head/{sniped}" } } }
            }, new JsonSerializerOptions { WriteIndented = true });
            if (config.SnipesharpServerWebhook) Send(config.snipesharpServerWebhook, json);
            if (!String.IsNullOrEmpty(config.DiscordWebhookUrl) && !String.IsNullOrEmpty(config.WebhookUsername)) Send(config.DiscordWebhookUrl, json);
        }
    }
}
