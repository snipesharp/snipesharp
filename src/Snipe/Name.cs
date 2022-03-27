using System.Net.Http.Headers;
using System.Text.Json;

namespace Snipe
{
    internal class Name
    {
        public static async Task Change(string name, bool prename=false) {
            try
            {
                var success = false;

                // prepare the http packet
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DataTypes.Account.v.Bearer.Trim());

                StringContent content = null!;
                if (prename) content = new StringContent(JsonSerializer.Serialize(new { profileName = name }));

                // get response and set packet sent time and reply time
                HttpResponseMessage response = prename
                    ? await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile", content)
                    : await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
                var sentDateValue = response.RequestMessage!.Headers!.Date!.Value;
                var receivedDateValue = response.Headers!.Date!.Value;
                string timeSent = $"sent@{sentDateValue.Hour}h{sentDateValue.Minute}m{sentDateValue.Second}s{sentDateValue.Millisecond}ms";
                string timeRecieved = $"reply@{receivedDateValue.Hour}h{receivedDateValue.Minute}m{receivedDateValue.Second}s{receivedDateValue.Millisecond}ms";

                // inform the user for the response
                var responseString = $"({(int)response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}";
                if (response.IsSuccessStatusCode) {
                    success = true;
                    Cli.Output.Success($"{responseString} [{timeSent}->{timeRecieved}] [sniped {name} using {(DataTypes.Account.v.Bearer.Length <= 6 ? DataTypes.Account.v.Bearer : ".." + DataTypes.Account.v.Bearer.Substring(DataTypes.Account.v.Bearer.Length - 6))}]");
                }
                else Cli.Output.Error($"{responseString} [{timeSent}->{timeRecieved}] [attempted sniping {name} using {(DataTypes.Account.v.Bearer.Length <= 6 ? DataTypes.Account.v.Bearer : ".." + DataTypes.Account.v.Bearer.Substring(DataTypes.Account.v.Bearer.Length - 6))}]");

                // post success
                if (success) {
                    Utils.Webhook.SendDiscordWebhooks(name);
                    if (DataTypes.Config.v.AutoSkinChange) Utils.Skin.Change(DataTypes.Config.v.SkinUrl, DataTypes.Config.v.SkinType, DataTypes.Account.v.Bearer);
                }
            }
            catch (Exception ex)
            {
                Cli.Output.ExitError($"Crashed while trying to change name:\n{ex.StackTrace}\n\n{ex.ToString()}");
                throw;
            }
        }
        protected static string GetResponseMessage(int code)
        {
            switch (code)
            {
                case 400: return "Name is invalid";
                case 403: return "Name is taken or has not become available";
                case 401: return "Bearer token expired or is not correct";
                case 429: return "Too many requests sent";
                case 500: return "API timed out";
                case 503: return "Service unavailable";
                case 200: return "Name changed";
                default: return "Unknown";
            }
        }
    }
}
