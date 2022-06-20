using System.Text.Json;
using DataTypes;

namespace Utils
{
    public class Webhook
    {
        public static async Task<HttpResponseMessage> Send(string webhookLink, string content) {
            try {
                HttpClient client = new HttpClient();
                StringContent stringContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookLink, stringContent);
                return response;
            }
            catch (Exception ex) {
                Cli.Output.Error(ex.ToString());
                return new HttpResponseMessage((System.Net.HttpStatusCode)400);
            }
        }
        public static async Task SendResultsWebhook(string content) {
            string json = JsonSerializer.Serialize(new {
                    avatar_url = "https://snipesharp.xyz/img/logo.png",
                    username = string.IsNullOrEmpty(DataTypes.Config.v.DiscordWebhookUsername) 
                        ? "snipesharp - snipe results" 
                        : DataTypes.Config.v.DiscordWebhookUsername + "'s snipe results",
                    content = content
                }
            );
            while (!(await Send(Config.v.ResultsWebhookUrl, json)).IsSuccessStatusCode) {
                await Task.Delay(5000);
            }
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
            if (Config.v.SnipesharpServerWebhook) Send(Config.v.snipesharpServerWebhook, jsonOfficial);

            // return if custom webhook isnt set
            if (string.IsNullOrEmpty(Config.v.CustomDiscordWebhookUrl)) return;
            
            // send custom webhook
            var searchesResponse = await Send("http://api.snipesharp.xyz:5150/searches", JsonSerializer.Serialize(new {name=sniped}));
            var searches = await searchesResponse.Content.ReadAsStringAsync();

            string json = JsonSerializer.Serialize(new {
                embeds = new[]
                {
                    new
                    {
                        title = "snipesharp snipe :tada:",
                        fields = new[] {
                            new {
                                name = "Searches",
                                value = $"{searches} per month",
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
