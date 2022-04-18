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
        
        public static void SendDiscordWebhooks(string sniped, bool prename=false) {
            string json = prename 
            ? JsonSerializer.Serialize(new
            { 
                name = sniped,
                sender = Config.v.DiscordWebhookUsername,
                prename = "true"
            }, new JsonSerializerOptions { WriteIndented = true })
            : JsonSerializer.Serialize(new
            { 
                name = sniped,
                sender = Config.v.DiscordWebhookUsername
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
