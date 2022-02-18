using System.Net.Http.Headers;

namespace Snipe
{
    internal class Name
    {
        public static async Task<HttpResponseMessage> Change(string name, string bearer) {
            // prepare the http packet and get the response
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer.Trim());
            HttpResponseMessage response = await client.PutAsync($"https://api.minecraftservices.com/minecraft/profile/name/{name}", null);
            
            // inform the user for the response
            var responseString = $"({(int)response.StatusCode} {response.StatusCode}) {GetResponseMessage((int)response.StatusCode)}";
            if (response.IsSuccessStatusCode) Cli.Output.Success(responseString);
            else Cli.Output.Error(responseString);

            // return
            return response;
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
