﻿using System.Net.Http.Headers;
using System.Text.Json;

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

                // wait for it...
                var snipeTime = Utils.Snipesharp.snipeTime;
                if ((DataTypes.Config.v.awaitFirstPacket && packetNumber != 1) || !DataTypes.Config.v.awaitFirstPacket) {
                    if (snipeTime.Second > DateTime.Now.Second || (snipeTime.Second == DateTime.Now.Second && snipeTime.Millisecond > DateTime.Now.Millisecond)) {
                        if (packetNumber == 0) while (snipeTime.Millisecond != DateTime.Now.Millisecond) {}
                        else while (snipeTime.AddMilliseconds(DataTypes.Config.v.PacketSpreadMs * packetNumber).Millisecond != DateTime.Now.Millisecond) {}
                    }
                    else await Task.Delay(DataTypes.Config.v.PacketSpreadMs * packetNumber);
                }

                // get response and set packet sent time and reply time
                var sentDateValue = DateTime.Now;
                HttpResponseMessage response = prename
                    ? await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile", content)
                    : await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
                var receivedDateValue = DateTime.Now;

                string timeSent = $"sent@{sentDateValue.Hour}h{sentDateValue.Minute}m{sentDateValue.Second}s{sentDateValue.Millisecond}ms";
                string timeRecieved = $"reply@{receivedDateValue.Hour}h{receivedDateValue.Minute}m{receivedDateValue.Second}s{receivedDateValue.Millisecond}ms";

                // inform the user for the response
                var responseString = $"({(int)response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}";
                var shortBearer = (DataTypes.Account.v.Bearer.Length <= 6 ? DataTypes.Account.v.Bearer : ".." + DataTypes.Account.v.Bearer.Substring(DataTypes.Account.v.Bearer.Length - 6));
                if (response.IsSuccessStatusCode) {
                    success = true;
                    Cli.Output.Success($"{responseString} [{timeSent}->{timeRecieved}] [sniped {name} using {shortBearer}]");
                }
                else Cli.Output.Error($"{responseString} [{timeSent}->{timeRecieved}] [attempted sniping {name} using {shortBearer}]");

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
