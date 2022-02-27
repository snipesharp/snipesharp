using System.Text.Json;
using DataTypes;

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
        
        public static void SendDiscordWebhooks(string sniped) {
            string json = JsonSerializer.Serialize(new
            { embeds = new[] { new {
                description = $"`{Config.v.DiscordWebhookUsername}` sniped [{sniped}](https://namemc.com/{sniped})",
                title = "snipesharp snipe :tada:",
                color = 1211647,
                image = new { url = $"https://mc-heads.net/head/{sniped}" } } }
            }, new JsonSerializerOptions { WriteIndented = true });
            if (Config.v.SnipesharpServerWebhook) 
                Send(Config.v.snipesharpServerWebhook, json);
            if (
                !String.IsNullOrEmpty(Config.v.CustomDiscordWebhookUrl) &&
                !String.IsNullOrEmpty(Config.v.DiscordWebhookUsername)
            )   Send(Config.v.CustomDiscordWebhookUrl, json);
        }
    }
}
