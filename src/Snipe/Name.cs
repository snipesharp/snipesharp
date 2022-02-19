using System.Net.Http.Headers;

namespace Snipe
{
    internal class Name
    {
        public static async Task<HttpResponseMessage> Change(string name, string bearer) {
            // prepare the http packet
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer.Trim());

            // get response and set packet sent time and reply time
            string timeSent = $"sent@{DateTime.Now.Hour}h{DateTime.Now.Minute}m{DateTime.Now.Second}s{DateTime.Now.Millisecond}ms";
            HttpResponseMessage response = await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
            string timeRecieved = $"reply@{DateTime.Now.Hour}h{DateTime.Now.Minute}m{DateTime.Now.Second}s{DateTime.Now.Millisecond}ms";

            // inform the user for the response
            var responseString = $"({(int)response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}";
            if (response.IsSuccessStatusCode) Cli.Output.Success($"{responseString} [{timeSent}->{timeRecieved}]");
            else Cli.Output.Error($"{responseString} [{timeSent}->{timeRecieved}]");

            // return
            return response;
        }
        protected static string GetResponseMessage(int code)
        {
            switch (code)
            {
                case 400: return "Name is invalid, longer than 16 characters or contains characters";
                case 403: return "Name is either taken or has not become available";
                case 401: return "Bearer token expired or is not correct";
                case 429: return "Too many requests sent";
                case 500: return "API Timed out";
                case 200: return "Name changed";
                default: return "Unknown";
            }
        }
    }
}
