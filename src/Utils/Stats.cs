using System.Text.Json;
using System.Net.Http.Headers;
using Cli.Animatables;

namespace Utils
{
    public class Stats
    {
        private static string NameChangeEndpoint = "https://api.minecraftservices.com/minecraft/profile/namechange";

        // <summary>Gets a UUID from username</summary>
        private static async Task<string> GetUUID(string username) {
            HttpClient client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(new string[] {username}), System.Text.Encoding.UTF8, "application/json");
            string json = await client.PostAsync($"https://api.mojang.com/profiles/minecraft", content).Result.Content.ReadAsStringAsync();
            try { return JsonSerializer.Deserialize<ProfileInformation[]>(json)![0].id!; } catch { return ""; }
        }

        /// <summary>Gets droptime for specified username</summary>
        public static async Task<Snipe.Droptime.UnixJSON> GetUnixDroptime(string username) {
            HttpClient client = new HttpClient();
            try {
                string uuid = await GetUUID(username);
                string responseJson = await client.GetAsync($"https://api.mojang.com/user/profile/{uuid}/names").Result.Content.ReadAsStringAsync();
                var profiles = JsonSerializer.Deserialize<ProfileResponse[]>(responseJson);

                // return 0 if the name is taken
                bool nameTaken = profiles[profiles.Length - 1].name.ToLower() == username.ToLower();
                if (nameTaken) return new Snipe.Droptime.UnixJSON { unix = 0 };

                long changedToAt = profiles[profiles.Length - 1].changedToAt;
                return new Snipe.Droptime.UnixJSON { unix = (changedToAt + 3196800000)/1000 };
            }
            catch { return new Snipe.Droptime.UnixJSON { unix = null }; }
        }
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static async Task<string?> GetUsername(string bearer) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearer}");

            var result = await client.GetAsync("https://api.minecraftservices.com/minecraft/profile");
            ProfileInformation? jsonResult = new ProfileInformation();
            try { jsonResult = JsonSerializer.Deserialize<ProfileInformation>(await result.Content.ReadAsStringAsync()); }
            catch (Exception e) { 
                Cli.Output.Error("Failed to get account username");
                FS.FileSystem.Log(e.ToString());
            }
            if (jsonResult != null) return jsonResult.name;
            return null;
        }
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
                    DateTime.TryParse(jsonRes.changedAt == null ? jsonRes.createdAt : jsonRes.changedAt, out DateTime changedAt);
                    var cooldownExpiryDate = changedAt.AddDays(30);

                    Cli.Output.ExitError($"{await GetUsername(bearer)} can't change name until {cooldownExpiryDate.ToString()}");
                }
                return jsonRes.nameChangeAllowed;
            }
            catch (Exception e){
                Console.WriteLine(e);
                FS.FileSystem.Log(e.ToString());
            }

            return false;
        }
        // return true if user owns minecraft, false otherwise
        public async static Task<bool> OwnsMinecraft(string bearer) {
            // create spinner
            Cli.Animatables.Spinner spinner = new Cli.Animatables.Spinner();
            // prepare http call using the bearer
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Cli.Templates.TWeb.UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            // get json response
            var mcOwnershipHttpResponse = await client.GetAsync("https://api.minecraftservices.com/entitlements/mcstore");
            if (!mcOwnershipHttpResponse.IsSuccessStatusCode) {
                spinner.Cancel();
                Cli.Output.ExitError(Cli.Templates.TAuth.AuthInforms.FailedBearer); // ! exits when using mojang bearer
            }
            var mcOwnershipJsonResponse = JsonSerializer.Deserialize<McOwnershipResponse>(
                await mcOwnershipHttpResponse.Content.ReadAsStringAsync()
            );

            // If doesn't own minecraft, prompt to redeem a giftcard 
            if (!Cli.Core.arguments.ContainsKey("--skip-gc-redeem") && (mcOwnershipJsonResponse.items == null || mcOwnershipJsonResponse.items.Length < 1)) {
                spinner.Cancel();
                var skipRedeem = Convert.ToBoolean(new SelectionPrompt("Your account doesn't own Minecraft, would you like to redeem a giftcard?",
                new string[] {
                    "Yes",
                    "No"
                }
                ).answerIndex);
                if (skipRedeem) return true;

                bool redeemResult;
                while (redeemResult = !await Snipe.Auth.RedeemGiftcard(Cli.Input.Request<string>(Cli.Templates.TRequests.Giftcode), bearer));
                return redeemResult;
            }
            spinner.Cancel();
            return true;
        }
        private struct UsernameInfo {
            public string status {get; set;}
        }
        private struct ProfileResponse {
            public string name {get; set;}
            public long changedToAt {get; set;}
        }
        public class ProfileInformation {
            public string? id { get; set; }
            public string? name { get; set; }
        }
        private struct Items {
            public string? name { get; set; }
            public string? signature { get; set; }
        }
        private struct McOwnershipResponse {
            public Items[] items { get; set; }
            public string signature { get; set; }
            public string keyId { get; set; }
        }

        private struct NameChangeResponse
        {
            public string changedAt { get; set; }
            public string createdAt { get; set; }
            public bool nameChangeAllowed { get; set; }
        }
    }
}