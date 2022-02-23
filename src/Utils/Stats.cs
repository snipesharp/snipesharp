using System.Text.Json;
using System.Net.Http.Headers;

namespace Utils
{
    public class Stats
    {
        private static string NameChangeEndpoint = "https://api.minecraftservices.com/minecraft/profile/namechange";
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static async Task<bool> CanChangeName(string bearer){
            HttpClient httpClient = new HttpClient();

            // prepare request headers
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearer);

            // return response or false
            try { 
                var res = await httpClient.GetStringAsync(NameChangeEndpoint);
                var jsonRes = JsonSerializer.Deserialize<NameChangeResponse>(res, JsonOptions);

                // exit if account cant change name
                if (!jsonRes.nameChangeAllowed) { 
                    DateTime.TryParse(jsonRes.changedAt, out DateTime changedAt);
                    var cooldownExpiryDate = changedAt.AddDays(30);
                    Cli.Output.ExitError($"Account can't change name until {cooldownExpiryDate.ToString()}");
                }
                return jsonRes.nameChangeAllowed;
            }
            catch (Exception e){
                Console.WriteLine(e);
            }

            return false;
        }

        // return true if user owns minecraft, false otherwise
        public async static Task<bool> OwnsMinecraft(string bearer) {
            // prepare http call using the bearer
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            // get json response
            var mcOwnershipHttpResponse = await client.GetAsync("https://api.minecraftservices.com/entitlements/mcstore");
            if (!mcOwnershipHttpResponse.IsSuccessStatusCode) Cli.Output.ExitError(Cli.Templates.TAuth.AuthInforms.FailedBearer);
            var mcOwnershipJsonResponse = JsonSerializer.Deserialize<McOwnershipResponse>(
                await mcOwnershipHttpResponse.Content.ReadAsStringAsync()
            );

            // If doesn't own minecraft, prompt to redeem a giftcard 
            if (mcOwnershipJsonResponse.items == null || mcOwnershipJsonResponse.items.Length < 1) {
                bool redeemResult;
                while (redeemResult = !await Snipe.Auth.RedeemGiftcard(Cli.Input.Request<string>(Cli.Templates.TRequests.Giftcode), bearer));
                return redeemResult;
            }
            return true;
        }
        public class Items {
            public string? name { get; set; }
            public string? signature { get; set; }
        }
        public struct McOwnershipResponse {
            public Items[] items { get; set; }
            public string signature { get; set; }
            public string keyId { get; set; }
        }

        public struct NameChangeResponse
        {
            public string changedAt { get; set; }
            public string createdAt { get; set; }
            public bool nameChangeAllowed { get; set; }
        }
    }
}