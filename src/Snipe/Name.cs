using System.Net.Http.Headers;
using System.Text.Json;
using DataTypes.SetText;

namespace Snipe
{
    internal class Name
    {
        public static async Task Change(string name, int packetNumber, bool prename=false) {
            try
            {
                var success = false;

                // prepare the http packet
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DataTypes.Account.v.Bearer.Trim());

                StringContent content = null!;
                if (prename) content = new StringContent(JsonSerializer.Serialize(new { profileName = name }));

                // wait for exact millisecond
                var snipeTime = Utils.Snipesharp.snipeTime;
                DateTime sentDateValue = new DateTime();
                DateTime receivedDateValue = new DateTime();
                HttpResponseMessage response = new HttpResponseMessage();
                await Task.Run(async () => {
                    // always enter on first packet
                    // if it isnt the first packet and awaitPackets is on, do NOT enter
                    // if awaitPackets isnt on, and awaitFirstPacket isnt on, enter
                    // if awaitFirstPacket is on and if we arent on the second packet enter
                    if (packetNumber == 0 || (!DataTypes.Config.v.awaitPackets && (!DataTypes.Config.v.awaitFirstPacket || (DataTypes.Config.v.awaitFirstPacket && packetNumber != 1)))) {
                        if (snipeTime.Second > DateTime.Now.Second || (snipeTime.Second == DateTime.Now.Second && snipeTime.Millisecond > DateTime.Now.Millisecond)) {
                            if (packetNumber == 0) while (snipeTime.Millisecond != DateTime.Now.Millisecond) {} 
                            else while (snipeTime.AddMilliseconds((DataTypes.Config.v.PacketSpreadMs * packetNumber)).Millisecond != DateTime.Now.Millisecond) {}
                        }
                        else await Task.Delay((DataTypes.Config.v.PacketSpreadMs * packetNumber));
                    }

                    // get response and set packet sent time and reply time
                    sentDateValue = DateTime.Now;
                    response = prename
                        ? await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile", content)
                        : await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
                    receivedDateValue = DateTime.Now;
                });

                // make sent & recieved strings
                string timeSent = $"{sentDateValue.Second}.{sentDateValue.Millisecond}s";
                string timeRecieved = $"{receivedDateValue.Second}.{(receivedDateValue.Millisecond)}s";

                // inform the user for the response
                var responseString = response.IsSuccessStatusCode
                    ? $"({SetText.Green}{(int)response.StatusCode}{SetText.ResetAll}) {GetResponseMessage((int)response.StatusCode)}"
                    : $"({SetText.Red}{(int)response.StatusCode}{SetText.ResetAll}) {GetResponseMessage((int)response.StatusCode)}";
                var shortBearer = (DataTypes.Account.v.Bearer.Length <= 6 ? DataTypes.Account.v.Bearer : ".." + DataTypes.Account.v.Bearer.Substring(DataTypes.Account.v.Bearer.Length - 6));
                if (response.IsSuccessStatusCode) {
                    success = true;
                    Cli.Output.Success($"{responseString} [recv @{SetText.Green}{timeSent}{SetText.ResetAll} -> recv @{SetText.Green}{timeRecieved}{SetText.ResetAll}] sniped {SetText.Blue}{name}{SetText.ResetAll} | {shortBearer}");
                }
                else Cli.Output.Error($"{responseString} [sent @{SetText.Blue}{timeSent}{SetText.ResetAll} -> recv @{SetText.Cyan}{timeRecieved}{SetText.ResetAll}] missed {SetText.Blue}{name}{SetText.ResetAll} | {shortBearer}");

                // post success
                if (success) {
                    Cli.Output.Inform($"{SetText.Gray}If you're happy with your snipe, consider supporting us by donating {SetText.White}@{SetText.Blue} https://snipesharp.xyz/donate");
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
                case 400: return "Invalid name";
                case 403: return "Name taken or not yet available";
                case 401: return "Bearer expired or incorrect";
                case 429: return "Too many requests";
                case 500: return "API timed out";
                case 503: return "Service unavailable";
                case 200: return "Name changed";
                default: return "Unknown";
            }
        }
    }
}
