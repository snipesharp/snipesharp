using System.Text.Json;
using System.Net.Http.Headers;

namespace Utils
{
    public class Stats
    {
        private static Func<string, string> UUIDEndpoint = (name) => $"https://api.mojang.com/users/profiles/minecraft/{name}";
        private static Func<string, string> HistroyEndpoint = (uuid) => $"https://api.mojang.com/user/profiles/{uuid}/names";
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public static async Task<bool> CanChangeName(string name){
            // get the uuid of the username
            var uuid = await GetUUID(name);
            if(uuid == null) return false;

            // get name history
            var nameHistory = await FetchHistory(uuid);
            if(nameHistory == null) return false;
            if(nameHistory.Count == 0) return false;

            // the latest name contains changedToAt
            var lastName = nameHistory.Last();

            // check if changedToAt was at least 30 days ago
            long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long diff = now - lastName.changedToAt;
            TimeSpan t = TimeSpan.FromMilliseconds(diff);
            Console.WriteLine(t.Days);
            return t.Days >= 30;
        }

        private static async Task<string>? GetUUID(string name){
            // prepare request
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // fetch name history and save
            try { 
                var res = await httpClient.GetStringAsync(UUIDEndpoint(name));
                return JsonSerializer.Deserialize<UUIDResponse>(res, JsonOptions).id;
            } catch { }

            return null;
        }

        private static async Task<List<NameHistoryItem>>? FetchHistory(string uuid){
            // prepare request
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // fetch name history and save
            try {
                var res = await httpClient.GetStringAsync(HistroyEndpoint(uuid));
                return JsonSerializer.Deserialize<List<NameHistoryItem>>(res, JsonOptions);
            } catch { }

            return null;
        }
    }

    public struct UUIDResponse {
        public string name { get; set; }
        public string id { get; set; }
    }

    public struct NameHistoryItem {
        public string name { get; set; }
        public long changedToAt { get; set; }
    }
}