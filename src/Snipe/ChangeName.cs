using System.Text.Json;

namespace Snipe
{
    internal class ChangeName
    {
        public static async Task<HttpResponseMessage> Change(string name, string bearer)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer.Trim());
                HttpResponseMessage response = await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
                if (response.IsSuccessStatusCode)
                    Cli.Output.Success($"({(int)response.StatusCode} {response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}");
                else
                    Cli.Output.Error($"({(int)response.StatusCode} {response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}");
                return response;
            }
        }
        protected static string GetResponseMessage(int code)
        {
            switch (code)
            {
                case 400: return "Name is invalid, longer than 16 characters or contains characters other than (a-zA-Z0-9_)";
                case 403: return "Name is either taken or has not become available";
                case 401: return "Bearer token expired or is not correct";
                case 429: return "Too many requests sent";
                case 500: return "Timed out (API lagged out and could not respond)";
                case 200: return "Name changed";
                default: return "Unknown";
            }
        }
    }
}
