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
                return jsonRes.nameChangeAllowed;
            }
            catch (Exception e){
                Console.WriteLine(e);
            }

            return false;
        }

        public struct NameChangeResponse
        {
            public string changedAt { get; set; }
            public string createdAt { get; set; }
            public bool nameChangeAllowed { get; set; }
        }
    }
}