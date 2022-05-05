using System.Text.Json;
using DataTypes;

namespace Utils
{
    public class Webhook
    {
        public static async Task<string> Send(string webhookLink, string content) {
            try {
                HttpClient client = new HttpClient();
                StringContent stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookLink, stringContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            catch (Exception ex) { Cli.Output.Error(ex.ToString()); return "null"; }
        }
        
        public static async Task SendDiscordWebhooks(string sniped, bool prename=false) {
            string jsonOfficial = prename 
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
                Send(Config.v.snipesharpServerWebhook, jsonOfficial);
            if (
                !String.IsNullOrEmpty(Config.v.CustomDiscordWebhookUrl) &&
                !String.IsNullOrEmpty(Config.v.DiscordWebhookUsername)
            ){
                var searchesResponse = await Send("http://webhooks.snipesharp.xyz/searches", JsonSerializer.Serialize(new {name=sniped}));
                string json = JsonSerializer.Serialize(new {
                    embeds = new[]
                    {
                        new
                        {
                            title = "snipesharp snipe :tada:",
                            fields = new[] {
                                new {
                                    name = "Searches",
                                    value = $"{searchesResponse} per month",
                                    inline = true
                                },
                                new {
                                    name = "Name sniped",
                                    value = $"[{sniped}](https://namemc.com/{sniped})",
                                    inline = true
                                },
                                new {
                                    name = "Sniped by",
                                    value = Config.v.DiscordWebhookUsername,
                                    inline = true
                                }
                            },
                            color = 1211647,
                            thumbnail = new
                            {
                                url = $"https://mc-heads.net/head/{sniped}"
                            }
                        }
                    }
                }, new JsonSerializerOptions { WriteIndented = true});
                Send(Config.v.CustomDiscordWebhookUrl, json);
            }
        }
    }
}
